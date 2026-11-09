namespace Boardy.MAUI.Servicios
{
    /// <summary>Resultado de una llamada a la API: éxito con datos, o un mensaje de error listo para mostrar.</summary>
    public class ResultadoApi
    {
        public bool Exito { get; init; }

        public string? Error { get; init; }

        public static ResultadoApi Ok() => new() { Exito = true };

        public static ResultadoApi Fallo(string error) => new() { Error = error };
    }

    public class ResultadoApi<T> : ResultadoApi
    {
        public T? Datos { get; init; }

        public static ResultadoApi<T> Ok(T? datos) => new() { Exito = true, Datos = datos };

        public static new ResultadoApi<T> Fallo(string error) => new() { Error = error };
    }
}
