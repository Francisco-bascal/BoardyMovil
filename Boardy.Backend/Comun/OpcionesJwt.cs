namespace Boardy.Backend.Comun
{
    /// <summary>Configuración de la sección "Jwt" de appsettings.</summary>
    public class OpcionesJwt
    {
        public const string Seccion = "Jwt";

        /// <summary>Nombre del claim que lleva el sello de seguridad del usuario.</summary>
        public const string ClaimSelloSeguridad = "sello";

        public string Emisor { get; set; } = string.Empty;

        public string Audiencia { get; set; } = string.Empty;

        /// <summary>Clave simétrica de firma: al menos 32 caracteres.</summary>
        public string Clave { get; set; } = string.Empty;

        public int DuracionHoras { get; set; } = 8;
    }
}
