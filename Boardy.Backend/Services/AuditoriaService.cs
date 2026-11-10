using System.Security.Claims;
using Boardy.Backend.Comun;
using Boardy.Backend.Data;
using Boardy.Backend.DTOs;
using Boardy.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Boardy.Backend.Services
{
    /// <summary>
    /// Registra y consulta las acciones realizadas en cada tablero.
    /// </summary>
    public class AuditoriaService : IAuditoriaService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditoriaService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        // Agrega la acción sin guardar: se persiste con el SaveChanges de la operación.
        public void Registrar(int tableroId, string accion)
        {
            var usuarioId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (usuarioId is null)
            {
                return;
            }

            _context.RegistrosAuditoria.Add(new RegistroAuditoria
            {
                TableroId = tableroId,
                UsuarioId = usuarioId,
                Accion = accion,
                Fecha = DateTime.UtcNow
            });
        }

        public async Task<Pagina<RegistroAuditoriaDto>> ListarAsync(int tableroId, string? usuarioId, int pagina, CancellationToken cancellationToken = default)
        {
            var numero = Math.Max(1, pagina);
            var registros = _context.RegistrosAuditoria
                .AsNoTracking()
                .Where(r => r.TableroId == tableroId && (string.IsNullOrEmpty(usuarioId) || r.UsuarioId == usuarioId));

            return new Pagina<RegistroAuditoriaDto>
            {
                Numero = numero,
                TotalElementos = await registros.CountAsync(cancellationToken),
                Elementos = await registros
                    .OrderByDescending(r => r.Fecha)
                    .Skip((numero - 1) * Pagina<RegistroAuditoriaDto>.Tamanio)
                    .Take(Pagina<RegistroAuditoriaDto>.Tamanio)
                    .Select(r => new RegistroAuditoriaDto
                    {
                        UsuarioNombre = r.Usuario.Nombre + " " + r.Usuario.Apellido,
                        Accion = r.Accion,
                        Fecha = r.Fecha
                    })
                    .ToListAsync(cancellationToken)
            };
        }
    }
}
