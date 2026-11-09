using Boardy.MAUI.Modelos;

namespace Boardy.MAUI.Servicios
{
    /// <summary>Tareas, su asignación de miembros y etiquetas, y sus comentarios.</summary>
    public class TareaServicio
    {
        private readonly ApiCliente _api;

        public TareaServicio(ApiCliente api)
        {
            _api = api;
        }

        public Task<ResultadoApi<List<TareaDto>>> ListarPorTableroAsync(int tableroId) => _api.GetAsync<List<TareaDto>>($"api/tableros/{tableroId}/tareas");

        public Task<ResultadoApi<TareaDto>> ObtenerAsync(int tareaId) => _api.GetAsync<TareaDto>($"api/tareas/{tareaId}");

        public Task<ResultadoApi<TareaDto>> CrearAsync(int listaTareaId, CrearTareaDto dto) => _api.PostAsync<TareaDto>($"api/listas/{listaTareaId}/tareas", dto);

        public Task<ResultadoApi<TareaDto>> ActualizarAsync(int tareaId, ActualizarTareaDto dto) => _api.PutAsync<TareaDto>($"api/tareas/{tareaId}", dto);

        public Task<ResultadoApi> EliminarAsync(int tareaId) => _api.DeleteAsync($"api/tareas/{tareaId}");

        public Task<ResultadoApi<TareaDto>> MoverAsync(int tareaId, MoverTareaDto dto) => _api.PutAsync<TareaDto>($"api/tareas/{tareaId}/mover", dto);

        public Task<ResultadoApi<TareaDto>> CambiarEstadoAsync(int tareaId, CambiarEstadoTareaDto dto) => _api.PutAsync<TareaDto>($"api/tareas/{tareaId}/estado", dto);

        public Task<ResultadoApi> AsignarEtiquetaAsync(int tareaId, int etiquetaId) => _api.PostAsync($"api/tareas/{tareaId}/etiquetas/{etiquetaId}");

        public Task<ResultadoApi> QuitarEtiquetaAsync(int tareaId, int etiquetaId) => _api.DeleteAsync($"api/tareas/{tareaId}/etiquetas/{etiquetaId}");

        public Task<ResultadoApi<TareaDto>> AsignarMiembroAsync(int tareaId, int tableroMiembroId) => _api.PostAsync<TareaDto>($"api/tareas/{tareaId}/miembros/{tableroMiembroId}");

        public Task<ResultadoApi> QuitarMiembroAsync(int tareaId, int tableroMiembroId) => _api.DeleteAsync($"api/tareas/{tareaId}/miembros/{tableroMiembroId}");

        public Task<ResultadoApi<List<ComentarioDto>>> ListarComentariosAsync(int tareaId) => _api.GetAsync<List<ComentarioDto>>($"api/tareas/{tareaId}/comentarios");

        public Task<ResultadoApi<ComentarioDto>> ComentarAsync(int tareaId, CrearComentarioDto dto) => _api.PostAsync<ComentarioDto>($"api/tareas/{tareaId}/comentarios", dto);

        public Task<ResultadoApi> EliminarComentarioAsync(int comentarioId) => _api.DeleteAsync($"api/comentarios/{comentarioId}");
    }
}
