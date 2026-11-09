namespace Boardy.MAUI.Servicios
{
    public static class ConfiguracionApi
    {
        // El emulador de Android ve el localhost de la PC como 10.0.2.2.
        // shortcut: URL fija de desarrollo (perfil "http" del backend); para un dispositivo físico o producción, cambiarla acá.
#if ANDROID
        public const string UrlBase = "http://10.0.2.2:5159/";
#else
        public const string UrlBase = "http://localhost:5159/";
#endif

        /// <summary>Convierte una ruta pública del backend (por ejemplo /uploads/x.png) en URL absoluta.</summary>
        public static string? UrlArchivo(string? ruta) =>
            string.IsNullOrEmpty(ruta) ? null : UrlBase.TrimEnd('/') + ruta;
    }
}
