using Boardy.Backend.Comun;
using Boardy.Backend.DTOs;

namespace Boardy.Backend.Services
{
    public interface IListaTareaService
    {
        Task<IReadOnlyList<ListaTareaDto>> ListarAsync(int tableroId, CancellationToken cancellationToken = default);

        Task<int?> ObtenerTableroIdAsync(int listaTareaId, CancellationToken cancellationToken = default);

        Task<int?> ObtenerEstadoAsync(int listaTareaId, CancellationToken cancellationToken = default);

        Task<ResultadoOperacion<ListaTareaDto>> CrearAsync(int tableroId, CrearListaTareaDto dto, CancellationToken cancellationToken = default);

        Task<ResultadoOperacion<ListaTareaDto>> ActualizarAsync(int listaTareaId, ActualizarListaTareaDto dto, CancellationToken cancellationToken = default);

        Task<ResultadoOperacion> ReordenarAsync(int listaTareaId, ReordenarDto dto, CancellationToken cancellationToken = default);

        Task<ResultadoOperacion> EliminarAsync(int listaTareaId, CancellationToken cancellationToken = default);
    }
}
