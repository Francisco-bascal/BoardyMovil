using Boardy.Backend.Comun;
using Boardy.Backend.DTOs;

namespace Boardy.Backend.Services
{
    public interface IRegistroErrorService
    {
        Task<Pagina<RegistroErrorDto>> ListarAsync(int pagina, CancellationToken cancellationToken = default);
    }
}
