using System.Text.Json;
using Boardy.MAUI.Modelos;

namespace Boardy.MAUI.Servicios
{
    /// <summary>
    /// Sesión del usuario: guarda el token y el perfil en SecureStorage para no pedir login en cada apertura.
    /// </summary>
    public class SesionServicio
    {
        private const string ClaveSesion = "boardy_sesion";

        private SesionDto? _sesion;

        public event Action? Cambio;

        public bool Activa => _sesion is not null && _sesion.Expiracion > DateTime.UtcNow;

        public string? Token => Activa ? _sesion!.Token : null;

        public PerfilDto? Usuario => _sesion?.Usuario;

        public async Task CargarAsync()
        {
            var json = await SecureStorage.Default.GetAsync(ClaveSesion);
            _sesion = json is null ? null : JsonSerializer.Deserialize<SesionDto>(json);
        }

        public async Task IniciarAsync(SesionDto sesion)
        {
            _sesion = sesion;
            await SecureStorage.Default.SetAsync(ClaveSesion, JsonSerializer.Serialize(sesion));
            Cambio?.Invoke();
        }

        /// <summary>Actualiza los datos del usuario mostrados en la app sin cambiar el token.</summary>
        public async Task ActualizarUsuarioAsync(PerfilDto usuario)
        {
            if (_sesion is null)
            {
                return;
            }

            _sesion.Usuario = usuario;
            await SecureStorage.Default.SetAsync(ClaveSesion, JsonSerializer.Serialize(_sesion));
            Cambio?.Invoke();
        }

        public void Cerrar()
        {
            _sesion = null;
            SecureStorage.Default.Remove(ClaveSesion);
            Cambio?.Invoke();
        }
    }
}
