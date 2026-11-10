using Boardy.Backend.Comun;
using Boardy.Backend.Data;
using Boardy.Backend.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Boardy.Backend.Services
{
    /// <summary>Consulta del log de errores para el administrador de la plataforma.</summary>
    public class RegistroErrorService : IRegistroErrorService
    {
        private readonly AppDbContext _context;

        public RegistroErrorService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Pagina<RegistroErrorDto>> ListarAsync(int pagina, CancellationToken cancellationToken = default)
        {
            var numero = Math.Max(1, pagina);

            return new Pagina<RegistroErrorDto>
            {
                Numero = numero,
                TotalElementos = await _context.RegistrosError.CountAsync(cancellationToken),
                Elementos = await _context.RegistrosError
                    .AsNoTracking()
                    .OrderByDescending(r => r.Fecha)
                    .Skip((numero - 1) * Pagina<RegistroErrorDto>.Tamanio)
                    .Take(Pagina<RegistroErrorDto>.Tamanio)
                    .Select(r => new RegistroErrorDto
                    {
                        Mensaje = r.Mensaje,
                        Detalle = r.Detalle,
                        Ruta = r.Ruta,
                        UsuarioId = r.UsuarioId,
                        Fecha = r.Fecha
                    })
                    .ToListAsync(cancellationToken)
            };
        }
    }
}
