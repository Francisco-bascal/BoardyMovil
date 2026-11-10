using Boardy.Backend.Autorizacion;
using Boardy.Backend.DTOs;
using Boardy.Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boardy.Backend.Controllers
{
    /// <summary>Estados de tarea propios de cada tablero.</summary>
    [Route("api")]
    public class EstadosTareaController : ControladorApiBase
    {
        private readonly IEstadoTareaService _estadoTareaService;

        public EstadosTareaController(IEstadoTareaService estadoTareaService, IAuthorizationService autorizacion)
            : base(autorizacion)
        {
            _estadoTareaService = estadoTareaService;
        }

        [HttpGet("tableros/{tableroId:int}/estados")]
        public async Task<ActionResult<IReadOnlyList<EstadoTareaDto>>> Listar(int tableroId, CancellationToken cancellationToken)
        {
            var acceso = await ValidarAccesoAsync(tableroId, PoliticasTablero.VerTablero);
            if (acceso is not null)
            {
                return acceso;
            }

            return Ok(await _estadoTareaService.ListarAsync(tableroId, cancellationToken));
        }

        [HttpPost("tableros/{tableroId:int}/estados")]
        public async Task<ActionResult<EstadoTareaDto>> Crear(int tableroId, CrearEstadoTareaDto dto, CancellationToken cancellationToken)
        {
            var acceso = await ValidarAccesoAsync(tableroId, PoliticasTablero.AdministrarTablero);
            if (acceso is not null)
            {
                return acceso;
            }

            return Responder(await _estadoTareaService.CrearAsync(tableroId, dto, cancellationToken));
        }

        [HttpPut("estados/{id:int}")]
        public async Task<ActionResult<EstadoTareaDto>> Actualizar(int id, ActualizarEstadoTareaDto dto, CancellationToken cancellationToken)
        {
            var acceso = await ValidarAccesoAsync(await _estadoTareaService.ObtenerTableroIdAsync(id, cancellationToken), PoliticasTablero.AdministrarTablero);
            if (acceso is not null)
            {
                return acceso;
            }

            return Responder(await _estadoTareaService.ActualizarAsync(id, dto, cancellationToken));
        }

        [HttpDelete("estados/{id:int}")]
        public async Task<ActionResult> Eliminar(int id, CancellationToken cancellationToken)
        {
            var acceso = await ValidarAccesoAsync(await _estadoTareaService.ObtenerTableroIdAsync(id, cancellationToken), PoliticasTablero.AdministrarTablero);
            if (acceso is not null)
            {
                return acceso;
            }

            return Responder(await _estadoTareaService.EliminarAsync(id, cancellationToken));
        }
    }
}
