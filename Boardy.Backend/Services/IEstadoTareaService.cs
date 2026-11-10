using Boardy.Backend.Comun;
using Boardy.Backend.DTOs;

namespace Boardy.Backend.Services
{
    public interface IEstadoTareaService
    {
        Task<IReadOnlyList<EstadoTareaDto>> ListarAsync(int tableroId, CancellationToken cancellationToken = default);

        Task<int?> ObtenerTableroIdAsync(int estadoTareaId, CancellationToken cancellationToken = default);

        Task<ResultadoOperacion<EstadoTareaDto>> CrearAsync(int tableroId, CrearEstadoTareaDto dto, CancellationToken cancellationToken = default);

        Task<ResultadoOperacion<EstadoTareaDto>> ActualizarAsync(int estadoTareaId, ActualizarEstadoTareaDto dto, CancellationToken cancellationToken = default);

        Task<ResultadoOperacion> EliminarAsync(int estadoTareaId, CancellationToken cancellationToken = default);
    }
}
