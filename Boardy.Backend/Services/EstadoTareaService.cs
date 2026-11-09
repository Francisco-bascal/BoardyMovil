using Boardy.Backend.Data;
using Boardy.Backend.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Boardy.Backend.Services
{
    public class EstadoTareaService : IEstadoTareaService
    {
        private readonly AppDbContext _context;

        public EstadoTareaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<EstadoTareaDto>> ListarAsync(CancellationToken cancellationToken = default)
        {
            return await _context.EstadoTareas
                .AsNoTracking()
                .OrderBy(e => e.EstadoTareaId)
                .Select(e => new EstadoTareaDto
                {
                    EstadoTareaId = e.EstadoTareaId,
                    Nombre = e.Nombre,
                    Descripcion = e.Descripcion,
                    CantidadTareas = e.Tareas.Count
                })
                .ToListAsync(cancellationToken);
        }
    }
}
