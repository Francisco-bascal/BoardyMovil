using Boardy.MAUI.Modelos;

namespace Boardy.MAUI.Servicios
{
    /// <summary>Miembros de un tablero e invitaciones.</summary>
    public class MiembroServicio
    {
        private readonly ApiCliente _api;

        public MiembroServicio(ApiCliente api)
        {
            _api = api;
        }

        public Task<ResultadoApi<List<TableroMiembroDto>>> ListarAsync(int tableroId) => _api.GetAsync<List<TableroMiembroDto>>($"api/tableros/{tableroId}/miembros");

        public Task<ResultadoApi<TableroMiembroDto>> CambiarRolAsync(int tableroId, int tableroMiembroId, int rolMiembroId) =>
            _api.PutAsync<TableroMiembroDto>($"api/tableros/{tableroId}/miembros/{tableroMiembroId}/rol", new AsignarRolDto { RolMiembroId = rolMiembroId });

        public Task<ResultadoApi> QuitarAsync(int tableroId, int tableroMiembroId) => _api.DeleteAsync($"api/tableros/{tableroId}/miembros/{tableroMiembroId}");

        public Task<ResultadoApi> TransferirPropiedadAsync(int tableroId, int tableroMiembroId) => _api.PostAsync($"api/tableros/{tableroId}/miembros/{tableroMiembroId}/transferir-propiedad");

        public Task<ResultadoApi<List<UsuarioBusquedaDto>>> BuscarUsuariosAsync(string texto) =>
            _api.GetAsync<List<UsuarioBusquedaDto>>($"api/usuarios/buscar?texto={Uri.EscapeDataString(texto)}");

        public Task<ResultadoApi<List<InvitacionTableroDto>>> ListarRecibidasAsync() => _api.GetAsync<List<InvitacionTableroDto>>("api/invitaciones");

        public Task<ResultadoApi<List<InvitacionTableroDto>>> ListarInvitacionesDelTableroAsync(int tableroId) => _api.GetAsync<List<InvitacionTableroDto>>($"api/tableros/{tableroId}/invitaciones");

        public Task<ResultadoApi<InvitacionTableroDto>> InvitarAsync(int tableroId, CrearInvitacionDto dto) => _api.PostAsync<InvitacionTableroDto>($"api/tableros/{tableroId}/invitaciones", dto);

        public Task<ResultadoApi> AceptarAsync(int invitacionId) => _api.PostAsync($"api/invitaciones/{invitacionId}/aceptar");

        public Task<ResultadoApi> RechazarAsync(int invitacionId) => _api.PostAsync($"api/invitaciones/{invitacionId}/rechazar");

        public Task<ResultadoApi> CancelarAsync(int invitacionId) => _api.PostAsync($"api/invitaciones/{invitacionId}/cancelar");
    }
}
