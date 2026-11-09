using System.Security.Claims;
using Boardy.Backend.Data;
using Boardy.Backend.Models;

namespace Boardy.Backend.Services
{
    /// <summary>
    /// Registra las acciones realizadas en cada tablero.
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
    }
}
