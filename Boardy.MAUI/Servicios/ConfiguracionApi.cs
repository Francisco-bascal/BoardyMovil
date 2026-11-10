namespace Boardy.MAUI.Servicios
{
    /// <summary>
    /// Dirección del backend. Se elige desde la pantalla "Servidor" y queda guardada en el dispositivo.
    /// </summary>
    public static class ConfiguracionApi
    {
        private const string ClaveUrl = "boardy_url_api";

        // El emulador de Android ve el localhost de la PC como 10.0.2.2.
#if ANDROID
        public const string UrlPorDefecto = "http://10.0.2.2:5159/";
#else
        public const string UrlPorDefecto = "http://localhost:5159/";
#endif

        /// <summary>Opciones rápidas de la pantalla de servidor (nombre, URL).</summary>
        public static readonly IReadOnlyList<(string Nombre, string Url)> Predefinidas = new[]
        {
            ("Emulador Android (PC local)", "http://10.0.2.2:5159/"),
            ("Teléfono por USB (adb reverse) / Windows", "http://localhost:5159/")
        };

        public static string UrlBase => Preferences.Default.Get(ClaveUrl, UrlPorDefecto);

        /// <summary>Guarda la URL normalizada (con barra final). Devuelve null si no es una URL http/https válida.</summary>
        public static string? Guardar(string url)
        {
            var texto = url.Trim();
            if (!texto.EndsWith('/'))
            {
                texto += "/";
            }

            if (!Uri.TryCreate(texto, UriKind.Absolute, out var uri) || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                return null;
            }

            Preferences.Default.Set(ClaveUrl, texto);
            return texto;
        }

        /// <summary>Convierte una ruta pública del backend (por ejemplo /uploads/x.png) en URL absoluta.</summary>
        public static string? UrlArchivo(string? ruta) =>
            string.IsNullOrEmpty(ruta) ? null : UrlBase.TrimEnd('/') + ruta;
    }
}
