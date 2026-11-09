using Boardy.Backend.Comun;
using Boardy.Backend.DTOs;

namespace Boardy.Backend.Services
{
    public interface IMiembroTableroService
    {
        Task<IReadOnlyList<TableroMiembroDto>> ListarAsync(int tableroId, CancellationToken cancellationToken = default);

        Task<ResultadoOperacion<TableroMiembroDto>> AgregarAsync(int tableroId, AgregarMiembroDto dto, CancellationToken cancellationToken = default);

        /// <summary>Cambia el rol de un miembro: el nuevo rol reemplaza al anterior (un único rol por miembro).</summary>
        Task<ResultadoOperacion<TableroMiembroDto>> CambiarRolAsync(int tableroId, int tableroMiembroId, AsignarRolDto dto, CancellationToken cancellationToken = default);

        Task<ResultadoOperacion> QuitarAsync(int tableroId, int tableroMiembroId, CancellationToken cancellationToken = default);
    }
}
