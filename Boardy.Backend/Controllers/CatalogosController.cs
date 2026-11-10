using Boardy.Backend.DTOs;
using Boardy.Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boardy.Backend.Controllers
{
    /// <summary>Catálogo global de roles de miembro: fijo, de solo lectura para todos.</summary>
    [Route("api/catalogos")]
    public class CatalogosController : ControladorApiBase
    {
        private readonly IRolMiembroService _rolMiembroService;

        public CatalogosController(IRolMiembroService rolMiembroService, IAuthorizationService autorizacion)
            : base(autorizacion)
        {
            _rolMiembroService = rolMiembroService;
        }

        [HttpGet("roles-miembro")]
        public async Task<ActionResult<IReadOnlyList<RolMiembroDto>>> ListarRoles(CancellationToken cancellationToken) =>
            Ok(await _rolMiembroService.ListarAsync(cancellationToken));
    }
}
