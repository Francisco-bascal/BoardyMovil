using Boardy.Backend.Comun;
using Boardy.Backend.Models;

namespace Boardy.Backend.Services
{
    public interface IFotoPerfilService
    {
        Task<ResultadoOperacion<string>> GuardarAsync(string usuarioId, IFormFile archivo, CancellationToken cancellationToken = default);

        Task<ResultadoOperacion> QuitarAsync(Usuario usuario);
    }
}
