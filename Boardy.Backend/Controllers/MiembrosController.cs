using Boardy.Backend.Autorizacion;
using Boardy.Backend.DTOs;
using Boardy.Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boardy.Backend.Controllers
{
    [Route("api/tableros/{tableroId:int}/miembros")]
    public class MiembrosController : ControladorApiBase
    {
        private readonly IMiembroTableroService _miembroService;
        private readonly ITableroService _tableroService;

        public MiembrosController(IMiembroTableroService miembroService, ITableroService tableroService, IAuthorizationService autorizacion)
            : base(autorizacion)
        {
            _miembroService = miembroService;
            _tableroService = tableroService;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<TableroMiembroDto>>> Listar(int tableroId, CancellationToken cancellationToken)
        {
            var acceso = await ValidarAccesoAsync(tableroId, PoliticasTablero.VerTablero);
            if (acceso is not null)
            {
                return acceso;
            }

            return Ok(await _miembroService.ListarAsync(tableroId, cancellationToken));
        }

        [HttpPost]
        public async Task<ActionResult<TableroMiembroDto>> Agregar(int tableroId, AgregarMiembroDto dto, CancellationToken cancellationToken)
        {
            var acceso = await ValidarAccesoAsync(tableroId, PoliticasTablero.AdministrarTablero);
            if (acceso is not null)
            {
                return acceso;
            }

            return Responder(await _miembroService.AgregarAsync(tableroId, dto, cancellationToken));
        }

        [HttpPut("{tableroMiembroId:int}/rol")]
        public async Task<ActionResult<TableroMiembroDto>> CambiarRol(int tableroId, int tableroMiembroId, AsignarRolDto dto, CancellationToken cancellationToken)
        {
            var acceso = await ValidarAccesoAsync(tableroId, PoliticasTablero.AdministrarTablero);
            if (acceso is not null)
            {
                return acceso;
            }

            return Responder(await _miembroService.CambiarRolAsync(tableroId, tableroMiembroId, dto, cancellationToken));
        }

        [HttpDelete("{tableroMiembroId:int}")]
        public async Task<ActionResult> Quitar(int tableroId, int tableroMiembroId, CancellationToken cancellationToken)
        {
            var acceso = await ValidarAccesoAsync(tableroId, PoliticasTablero.AdministrarTablero);
            if (acceso is not null)
            {
                return acceso;
            }

            return Responder(await _miembroService.QuitarAsync(tableroId, tableroMiembroId, cancellationToken));
        }

        [HttpPost("{tableroMiembroId:int}/transferir-propiedad")]
        public async Task<ActionResult> TransferirPropiedad(int tableroId, int tableroMiembroId, CancellationToken cancellationToken)
        {
            var acceso = await ValidarAccesoAsync(tableroId, PoliticasTablero.GestionarTablero);
            if (acceso is not null)
            {
                return acceso;
            }

            return Responder(await _tableroService.TransferirPropiedadAsync(tableroId, tableroMiembroId, UsuarioId, cancellationToken));
        }
    }
}
