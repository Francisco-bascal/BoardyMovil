using Boardy.Backend.Autorizacion;
using Boardy.Backend.Comun;
using Boardy.Backend.DTOs;
using Boardy.Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boardy.Backend.Controllers
{
    /// <summary>Auditoría de un tablero: solo la ven su Propietario y sus Administradores.</summary>
    [Route("api/tableros/{tableroId:int}/auditoria")]
    public class AuditoriaController : ControladorApiBase
    {
        private readonly IAuditoriaService _auditoriaService;

        public AuditoriaController(IAuditoriaService auditoriaService, IAuthorizationService autorizacion)
            : base(autorizacion)
        {
            _auditoriaService = auditoriaService;
        }

        [HttpGet]
        public async Task<ActionResult<Pagina<RegistroAuditoriaDto>>> Listar(int tableroId, string? usuarioId, int pagina = 1, CancellationToken cancellationToken = default)
        {
            var acceso = await ValidarAccesoAsync(tableroId, PoliticasTablero.AdministrarTablero);
            if (acceso is not null)
            {
                return acceso;
            }

            return Ok(await _auditoriaService.ListarAsync(tableroId, usuarioId, pagina, cancellationToken));
        }
    }
}
