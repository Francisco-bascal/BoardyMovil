using Boardy.Backend.DTOs;
using Boardy.Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boardy.Backend.Controllers
{
    /// <summary>Catálogos globales de solo lectura (su administración queda en la versión web).</summary>
    [Route("api/catalogos")]
    public class CatalogosController : ControladorApiBase
    {
        private readonly IEstadoTareaService _estadoTareaService;
        private readonly IRolMiembroService _rolMiembroService;

        public CatalogosController(IEstadoTareaService estadoTareaService, IRolMiembroService rolMiembroService, IAuthorizationService autorizacion)
            : base(autorizacion)
        {
            _estadoTareaService = estadoTareaService;
            _rolMiembroService = rolMiembroService;
        }

        [HttpGet("estados-tarea")]
        public async Task<ActionResult<IReadOnlyList<EstadoTareaDto>>> ListarEstados(CancellationToken cancellationToken) =>
            Ok(await _estadoTareaService.ListarAsync(cancellationToken));

        [HttpGet("roles-miembro")]
        public async Task<ActionResult<IReadOnlyList<RolMiembroDto>>> ListarRoles(CancellationToken cancellationToken) =>
            Ok(await _rolMiembroService.ListarAsync(cancellationToken));
    }
}
