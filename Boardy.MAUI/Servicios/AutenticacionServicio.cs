using Boardy.MAUI.Modelos;

namespace Boardy.MAUI.Servicios
{
    public class AutenticacionServicio
    {
        private readonly ApiCliente _api;
        private readonly SesionServicio _sesion;

        public AutenticacionServicio(ApiCliente api, SesionServicio sesion)
        {
            _api = api;
            _sesion = sesion;
        }

        public Task<ResultadoApi> IniciarSesionAsync(IniciarSesionDto dto) =>
            AbrirSesionAsync(_api.PostAsync<SesionDto>("api/autenticacion/inicio-sesion", dto));

        public Task<ResultadoApi> RegistrarAsync(RegistrarUsuarioDto dto) =>
            AbrirSesionAsync(_api.PostAsync<SesionDto>("api/autenticacion/registro", dto));

        public void CerrarSesion() => _sesion.Cerrar();

        private async Task<ResultadoApi> AbrirSesionAsync(Task<ResultadoApi<SesionDto>> llamada)
        {
            var resultado = await llamada;
            if (resultado.Exito)
            {
                await _sesion.IniciarAsync(resultado.Datos!);
            }

            return resultado;
        }
    }
}
