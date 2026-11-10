using Boardy.MAUI.Modelos;

namespace Boardy.MAUI.Servicios
{
    /// <summary>Perfil propio y administración de usuarios de la plataforma.</summary>
    public class UsuarioServicio
    {
        private readonly ApiCliente _api;
        private readonly SesionServicio _sesion;

        public UsuarioServicio(ApiCliente api, SesionServicio sesion)
        {
            _api = api;
            _sesion = sesion;
        }

        public Task<ResultadoApi<PerfilDto>> ObtenerPerfilAsync() => _api.GetAsync<PerfilDto>("api/perfil");

        public async Task<ResultadoApi<PerfilDto>> ActualizarPerfilAsync(ActualizarPerfilDto dto)
        {
            var resultado = await _api.PutAsync<PerfilDto>("api/perfil", dto);
            if (resultado.Exito)
            {
                await _sesion.ActualizarUsuarioAsync(resultado.Datos!);
            }

            return resultado;
        }

        public Task<ResultadoApi> CambiarContrasenaAsync(CambiarContrasenaDto dto) => _api.PutAsync("api/perfil/contrasena", dto);

        public async Task<ResultadoApi<string>> SubirFotoAsync(Stream contenido, string nombreArchivo)
        {
            var resultado = await _api.PutArchivoAsync<string>("api/perfil/foto", contenido, nombreArchivo, "archivo");
            if (resultado.Exito && _sesion.Usuario is not null)
            {
                _sesion.Usuario.FotoPerfil = resultado.Datos;
                await _sesion.ActualizarUsuarioAsync(_sesion.Usuario);
            }

            return resultado;
        }

        public async Task<ResultadoApi> EliminarCuentaAsync()
        {
            var resultado = await _api.DeleteAsync("api/perfil");
            if (resultado.Exito)
            {
                _sesion.Cerrar();
            }

            return resultado;
        }

        public Task<ResultadoApi<List<UsuarioAdministracionDto>>> ListarAsync() => _api.GetAsync<List<UsuarioAdministracionDto>>("api/usuarios");

        public Task<ResultadoApi<UsuarioAdministracionDto>> ObtenerAsync(string usuarioId) => _api.GetAsync<UsuarioAdministracionDto>($"api/usuarios/{usuarioId}");

        public Task<ResultadoApi<UsuarioAdministracionDto>> CrearAsync(RegistrarUsuarioDto dto) => _api.PostAsync<UsuarioAdministracionDto>("api/usuarios", dto);

        public Task<ResultadoApi<UsuarioAdministracionDto>> EditarAsync(string usuarioId, EditarUsuarioDto dto) => _api.PutAsync<UsuarioAdministracionDto>($"api/usuarios/{usuarioId}", dto);

        public Task<ResultadoApi> AlternarEstadoAsync(string usuarioId) => _api.PostAsync($"api/usuarios/{usuarioId}/alternar-estado");

        public Task<ResultadoApi<Pagina<RegistroErrorDto>>> ListarErroresAsync(int pagina) =>
            _api.GetAsync<Pagina<RegistroErrorDto>>($"api/plataforma/errores?pagina={pagina}");

        public Task<ResultadoApi> AlternarAdministradorAsync(string usuarioId) => _api.PostAsync($"api/usuarios/{usuarioId}/alternar-administrador");
    }
}
