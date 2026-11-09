using Boardy.Backend.Autorizacion;
using Boardy.Backend.DTOs;
using Boardy.Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boardy.Backend.Controllers
{
    [Route("api")]
    public class InvitacionesController : ControladorApiBase
    {
        private readonly IInvitacionService _invitacionService;

        public InvitacionesController(IInvitacionService invitacionService, IAuthorizationService autorizacion)
            : base(autorizacion)
        {
            _invitacionService = invitacionService;
        }

        /// <summary>Invitaciones pendientes recibidas por el usuario actual.</summary>
        [HttpGet("invitaciones")]
        public async Task<ActionResult<IReadOnlyList<InvitacionTableroDto>>> ListarRecibidas(CancellationToken cancellationToken) =>
            Ok(await _invitacionService.ListarRecibidasAsync(UsuarioId, cancellationToken));

        [HttpGet("tableros/{tableroId:int}/invitaciones")]
        public async Task<ActionResult<IReadOnlyList<InvitacionTableroDto>>> ListarDelTablero(int tableroId, CancellationToken cancellationToken)
        {
            var acceso = await ValidarAccesoAsync(tableroId, PoliticasTablero.AdministrarTablero);
            if (acceso is not null)
            {
                return acceso;
            }

            return Ok(await _invitacionService.ListarDelTableroAsync(tableroId, cancellationToken));
        }

        [HttpPost("tableros/{tableroId:int}/invitaciones")]
        public async Task<ActionResult<InvitacionTableroDto>> Crear(int tableroId, CrearInvitacionDto dto, CancellationToken cancellationToken)
        {
            var acceso = await ValidarAccesoAsync(tableroId, PoliticasTablero.AdministrarTablero);
            if (acceso is not null)
            {
                return acceso;
            }

            return Responder(await _invitacionService.CrearAsync(tableroId, dto, UsuarioId, cancellationToken));
        }

        [HttpPost("invitaciones/{id:int}/aceptar")]
        public async Task<ActionResult> Aceptar(int id, CancellationToken cancellationToken) =>
            Responder(await _invitacionService.AceptarAsync(id, UsuarioId, cancellationToken));

        [HttpPost("invitaciones/{id:int}/rechazar")]
        public async Task<ActionResult> Rechazar(int id, CancellationToken cancellationToken) =>
            Responder(await _invitacionService.RechazarAsync(id, UsuarioId, cancellationToken));

        [HttpPost("invitaciones/{id:int}/cancelar")]
        public async Task<ActionResult> Cancelar(int id, CancellationToken cancellationToken)
        {
            var acceso = await ValidarAccesoAsync(await _invitacionService.ObtenerTableroIdAsync(id, cancellationToken), PoliticasTablero.AdministrarTablero);
            if (acceso is not null)
            {
                return acceso;
            }

            return Responder(await _invitacionService.CancelarAsync(id, cancellationToken));
        }
    }
}
