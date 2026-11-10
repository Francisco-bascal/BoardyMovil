using Boardy.MAUI.Modelos;

namespace Boardy.MAUI.Servicios
{
    /// <summary>Tableros, sus listas, estados, etiquetas y auditoría, y el catálogo de roles.</summary>
    public class TableroServicio
    {
        private readonly ApiCliente _api;

        public TableroServicio(ApiCliente api)
        {
            _api = api;
        }

        public Task<ResultadoApi<List<TableroDto>>> ListarAsync() => _api.GetAsync<List<TableroDto>>("api/tableros");

        public Task<ResultadoApi<TableroDetalleDto>> ObtenerAsync(int tableroId) => _api.GetAsync<TableroDetalleDto>($"api/tableros/{tableroId}");

        public Task<ResultadoApi<TableroDto>> CrearAsync(CrearTableroDto dto) => _api.PostAsync<TableroDto>("api/tableros", dto);

        public Task<ResultadoApi<TableroDto>> ActualizarAsync(int tableroId, ActualizarTableroDto dto) => _api.PutAsync<TableroDto>($"api/tableros/{tableroId}", dto);

        public Task<ResultadoApi> EliminarAsync(int tableroId) => _api.DeleteAsync($"api/tableros/{tableroId}");

        public Task<ResultadoApi<List<ListaTareaDto>>> ListarListasAsync(int tableroId) => _api.GetAsync<List<ListaTareaDto>>($"api/tableros/{tableroId}/listas");

        public Task<ResultadoApi<ListaTareaDto>> CrearListaAsync(int tableroId, CrearListaTareaDto dto) => _api.PostAsync<ListaTareaDto>($"api/tableros/{tableroId}/listas", dto);

        public Task<ResultadoApi<ListaTareaDto>> ActualizarListaAsync(int listaTareaId, ActualizarListaTareaDto dto) => _api.PutAsync<ListaTareaDto>($"api/listas/{listaTareaId}", dto);

        public Task<ResultadoApi> EliminarListaAsync(int listaTareaId) => _api.DeleteAsync($"api/listas/{listaTareaId}");

        public Task<ResultadoApi<List<EtiquetaDto>>> ListarEtiquetasAsync(int tableroId) => _api.GetAsync<List<EtiquetaDto>>($"api/tableros/{tableroId}/etiquetas");

        public Task<ResultadoApi<EtiquetaDto>> CrearEtiquetaAsync(int tableroId, CrearEtiquetaDto dto) => _api.PostAsync<EtiquetaDto>($"api/tableros/{tableroId}/etiquetas", dto);

        public Task<ResultadoApi> EliminarEtiquetaAsync(int etiquetaId) => _api.DeleteAsync($"api/etiquetas/{etiquetaId}");

        public Task<ResultadoApi<List<EstadoTareaDto>>> ListarEstadosAsync(int tableroId) => _api.GetAsync<List<EstadoTareaDto>>($"api/tableros/{tableroId}/estados");

        public Task<ResultadoApi<EstadoTareaDto>> CrearEstadoAsync(int tableroId, CrearEstadoTareaDto dto) => _api.PostAsync<EstadoTareaDto>($"api/tableros/{tableroId}/estados", dto);

        public Task<ResultadoApi<EstadoTareaDto>> ActualizarEstadoAsync(int estadoTareaId, ActualizarEstadoTareaDto dto) => _api.PutAsync<EstadoTareaDto>($"api/estados/{estadoTareaId}", dto);

        public Task<ResultadoApi> EliminarEstadoAsync(int estadoTareaId) => _api.DeleteAsync($"api/estados/{estadoTareaId}");

        public Task<ResultadoApi<Pagina<RegistroAuditoriaDto>>> ListarAuditoriaAsync(int tableroId, int pagina) =>
            _api.GetAsync<Pagina<RegistroAuditoriaDto>>($"api/tableros/{tableroId}/auditoria?pagina={pagina}");

        public Task<ResultadoApi<List<RolMiembroDto>>> ListarRolesAsync() => _api.GetAsync<List<RolMiembroDto>>("api/catalogos/roles-miembro");
    }
}
