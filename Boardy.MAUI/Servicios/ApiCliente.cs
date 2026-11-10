using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Boardy.MAUI.Servicios
{
    /// <summary>
    /// Cliente HTTP de la API: agrega el token, traduce las respuestas de error (ProblemDetails)
    /// a mensajes y cierra la sesión si el backend responde 401.
    /// </summary>
    public class ApiCliente
    {
        public static readonly JsonSerializerOptions OpcionesJson = new(JsonSerializerDefaults.Web)
        {
            Converters = { new JsonStringEnumConverter() }
        };

        private readonly HttpClient _http;
        private readonly SesionServicio _sesion;

        public ApiCliente(HttpClient http, SesionServicio sesion)
        {
            _http = http;
            _sesion = sesion;
        }

        public Task<ResultadoApi<T>> GetAsync<T>(string url) =>
            EnviarAsync<T>(new HttpRequestMessage(HttpMethod.Get, url));

        public Task<ResultadoApi<T>> PostAsync<T>(string url, object? cuerpo = null) =>
            EnviarAsync<T>(ConCuerpo(HttpMethod.Post, url, cuerpo));

        public Task<ResultadoApi<T>> PutAsync<T>(string url, object? cuerpo = null) =>
            EnviarAsync<T>(ConCuerpo(HttpMethod.Put, url, cuerpo));

        public async Task<ResultadoApi> PostAsync(string url, object? cuerpo = null) =>
            await EnviarAsync<object>(ConCuerpo(HttpMethod.Post, url, cuerpo));

        public async Task<ResultadoApi> PutAsync(string url, object? cuerpo = null) =>
            await EnviarAsync<object>(ConCuerpo(HttpMethod.Put, url, cuerpo));

        public async Task<ResultadoApi> DeleteAsync(string url) =>
            await EnviarAsync<object>(new HttpRequestMessage(HttpMethod.Delete, url));

        public Task<ResultadoApi<T>> PutArchivoAsync<T>(string url, Stream contenido, string nombreArchivo, string campo)
        {
            var formulario = new MultipartFormDataContent { { new StreamContent(contenido), campo, nombreArchivo } };
            return EnviarAsync<T>(new HttpRequestMessage(HttpMethod.Put, url) { Content = formulario });
        }

        private static HttpRequestMessage ConCuerpo(HttpMethod metodo, string url, object? cuerpo) => new(metodo, url)
        {
            Content = cuerpo is null ? null : JsonContent.Create(cuerpo, cuerpo.GetType(), options: OpcionesJson)
        };

        private async Task<ResultadoApi<T>> EnviarAsync<T>(HttpRequestMessage solicitud)
        {
            // La URL base se resuelve en cada llamada: puede cambiarse desde la pantalla "Servidor".
            solicitud.RequestUri = new Uri(new Uri(ConfiguracionApi.UrlBase), solicitud.RequestUri!.OriginalString);

            if (_sesion.Token is not null)
            {
                solicitud.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _sesion.Token);
            }

            HttpResponseMessage respuesta;
            try
            {
                respuesta = await _http.SendAsync(solicitud);
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                const string sinConexion = "No se pudo conectar con el servidor. Revisá tu conexión o cambiá la URL del servidor.";

                // Con sesión iniciada se vuelve al login, que es donde se puede cambiar la URL.
                // shortcut: un solo corte (o una respuesta que supera el timeout) ya cierra la sesión; si molesta, contar fallos seguidos.
                if (_sesion.Token is not null)
                {
                    _sesion.Cerrar(sinConexion);
                }

                return ResultadoApi<T>.Fallo(sinConexion);
            }

            using (respuesta)
            {
                if (respuesta.IsSuccessStatusCode)
                {
                    var datos = respuesta.StatusCode == HttpStatusCode.NoContent || typeof(T) == typeof(object)
                        ? default
                        : await respuesta.Content.ReadFromJsonAsync<T>(OpcionesJson);
                    return ResultadoApi<T>.Ok(datos);
                }

                if (respuesta.StatusCode == HttpStatusCode.Unauthorized && _sesion.Token is not null)
                {
                    const string sesionVencida = "Tu sesión venció. Iniciá sesión de nuevo.";
                    _sesion.Cerrar(sesionVencida);
                    return ResultadoApi<T>.Fallo(sesionVencida);
                }

                return ResultadoApi<T>.Fallo(await LeerErrorAsync(respuesta));
            }
        }

        private static async Task<string> LeerErrorAsync(HttpResponseMessage respuesta)
        {
            try
            {
                var problema = await respuesta.Content.ReadFromJsonAsync<DetalleProblema>(OpcionesJson);

                if (problema?.Errors is { Count: > 0 })
                {
                    return string.Join(" ", problema.Errors.Values.SelectMany(m => m));
                }

                if (!string.IsNullOrWhiteSpace(problema?.Detail))
                {
                    return problema.Detail;
                }
            }
            catch (JsonException)
            {
                // Respuesta sin cuerpo JSON (por ejemplo un 403 vacío): se usa el mensaje por código.
            }

            return respuesta.StatusCode switch
            {
                HttpStatusCode.Forbidden => "No tenés permiso para realizar esta acción.",
                HttpStatusCode.NotFound => "No se encontró lo que buscabas.",
                _ => "Ocurrió un error inesperado. Probá de nuevo."
            };
        }

        private class DetalleProblema
        {
            public string? Detail { get; set; }

            public Dictionary<string, string[]>? Errors { get; set; }
        }
    }
}
