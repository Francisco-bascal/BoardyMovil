using Boardy.Backend.Autorizacion;
using Boardy.Backend.DTOs;
using Boardy.Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boardy.Backend.Controllers
{
    [Route("api")]
    public class EtiquetasController : ControladorApiBase
    {
        private readonly IEtiquetaService _etiquetaService;

        public EtiquetasController(IEtiquetaService etiquetaService, IAuthorizationService autorizacion)
            : base(autorizacion)
        {
            _etiquetaService = etiquetaService;
        }

        [HttpGet("tableros/{tableroId:int}/etiquetas")]
        public async Task<ActionResult<IReadOnlyList<EtiquetaDto>>> Listar(int tableroId, CancellationToken cancellationToken)
        {
            var acceso = await ValidarAccesoAsync(tableroId, PoliticasTablero.VerTablero);
            if (acceso is not null)
            {
                return acceso;
            }

            return Ok(await _etiquetaService.ListarAsync(tableroId, cancellationToken));
        }

        [HttpPost("tableros/{tableroId:int}/etiquetas")]
        public async Task<ActionResult<EtiquetaDto>> Crear(int tableroId, CrearEtiquetaDto dto, CancellationToken cancellationToken)
        {
            var acceso = await ValidarAccesoAsync(tableroId, PoliticasTablero.AdministrarTablero);
            if (acceso is not null)
            {
                return acceso;
            }

            return Responder(await _etiquetaService.CrearAsync(tableroId, dto, cancellationToken));
        }

        [HttpPut("etiquetas/{id:int}")]
        public async Task<ActionResult<EtiquetaDto>> Actualizar(int id, ActualizarEtiquetaDto dto, CancellationToken cancellationToken)
        {
            var acceso = await ValidarAccesoAsync(await _etiquetaService.ObtenerTableroIdAsync(id, cancellationToken), PoliticasTablero.AdministrarTablero);
            if (acceso is not null)
            {
                return acceso;
            }

            return Responder(await _etiquetaService.ActualizarAsync(id, dto, cancellationToken));
        }

        [HttpDelete("etiquetas/{id:int}")]
        public async Task<ActionResult> Eliminar(int id, CancellationToken cancellationToken)
        {
            var acceso = await ValidarAccesoAsync(await _etiquetaService.ObtenerTableroIdAsync(id, cancellationToken), PoliticasTablero.AdministrarTablero);
            if (acceso is not null)
            {
                return acceso;
            }

            return Responder(await _etiquetaService.EliminarAsync(id, cancellationToken));
        }
    }
}
