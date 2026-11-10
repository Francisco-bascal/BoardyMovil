using Boardy.Backend.Autorizacion;
using Boardy.Backend.Comun;
using Boardy.Backend.DTOs;
using Boardy.Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boardy.Backend.Controllers
{
    /// <summary>Herramientas del administrador de la plataforma.</summary>
    [Route("api/plataforma")]
    [Authorize(Roles = RolesGlobales.Administrador)]
    public class PlataformaController : ControladorApiBase
    {
        private readonly IRegistroErrorService _registroErrorService;

        public PlataformaController(IRegistroErrorService registroErrorService, IAuthorizationService autorizacion)
            : base(autorizacion)
        {
            _registroErrorService = registroErrorService;
        }

        [HttpGet("errores")]
        public async Task<ActionResult<Pagina<RegistroErrorDto>>> ListarErrores(int pagina = 1, CancellationToken cancellationToken = default) =>
            Ok(await _registroErrorService.ListarAsync(pagina, cancellationToken));
    }
}
