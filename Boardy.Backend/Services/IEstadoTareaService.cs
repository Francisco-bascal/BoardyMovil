using Boardy.Backend.DTOs;

namespace Boardy.Backend.Services
{
    public interface IEstadoTareaService
    {
        Task<IReadOnlyList<EstadoTareaDto>> ListarAsync(CancellationToken cancellationToken = default);
    }
}
