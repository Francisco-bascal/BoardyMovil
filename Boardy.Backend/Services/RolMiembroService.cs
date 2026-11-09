using Boardy.Backend.Data;
using Boardy.Backend.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Boardy.Backend.Services
{
    public class RolMiembroService : IRolMiembroService
    {
        private readonly AppDbContext _context;

        public RolMiembroService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<RolMiembroDto>> ListarAsync(CancellationToken cancellationToken = default)
        {
            return await _context.RolMiembros
                .AsNoTracking()
                .OrderBy(e => e.RolMiembroId)
                .Select(e => new RolMiembroDto
                {
                    RolMiembroId = e.RolMiembroId,
                    Nombre = e.Nombre,
                    Descripcion = e.Descripcion,
                    CantidadMiembros = e.Miembros.Count
                })
                .ToListAsync(cancellationToken);
        }
    }
}
