using Boardy.Backend.DTOs;

namespace Boardy.Backend.Services
{
    public interface IRolMiembroService
    {
        Task<IReadOnlyList<RolMiembroDto>> ListarAsync(CancellationToken cancellationToken = default);
    }
}
