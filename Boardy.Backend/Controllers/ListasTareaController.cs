using Boardy.Backend.Autorizacion;
using Boardy.Backend.DTOs;
using Boardy.Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boardy.Backend.Controllers
{
    [Route("api")]
    public class ListasTareaController : ControladorApiBase
    {
        private readonly IListaTareaService _listaTareaService;

        public ListasTareaController(IListaTareaService listaTareaService, IAuthorizationService autorizacion)
            : base(autorizacion)
        {
            _listaTareaService = listaTareaService;
        }

        [HttpGet("tableros/{tableroId:int}/listas")]
        public async Task<ActionResult<IReadOnlyList<ListaTareaDto>>> Listar(int tableroId, CancellationToken cancellationToken)
        {
            var acceso = await ValidarAccesoAsync(tableroId, PoliticasTablero.VerTablero);
            if (acceso is not null)
            {
                return acceso;
            }

            return Ok(await _listaTareaService.ListarAsync(tableroId, cancellationToken));
        }

        [HttpPost("tableros/{tableroId:int}/listas")]
        public async Task<ActionResult<ListaTareaDto>> Crear(int tableroId, CrearListaTareaDto dto, CancellationToken cancellationToken)
        {
            var acceso = await ValidarAccesoAsync(tableroId, PoliticasTablero.AdministrarTablero);
            if (acceso is not null)
            {
                return acceso;
            }

            return Responder(await _listaTareaService.CrearAsync(tableroId, dto, cancellationToken));
        }

        [HttpPut("listas/{id:int}")]
        public async Task<ActionResult<ListaTareaDto>> Actualizar(int id, ActualizarListaTareaDto dto, CancellationToken cancellationToken)
        {
            var acceso = await ValidarAccesoAsync(await _listaTareaService.ObtenerTableroIdAsync(id, cancellationToken), PoliticasTablero.AdministrarTablero);
            if (acceso is not null)
            {
                return acceso;
            }

            return Responder(await _listaTareaService.ActualizarAsync(id, dto, cancellationToken));
        }

        [HttpPut("listas/{id:int}/orden")]
        public async Task<ActionResult> Reordenar(int id, ReordenarDto dto, CancellationToken cancellationToken)
        {
            var acceso = await ValidarAccesoAsync(await _listaTareaService.ObtenerTableroIdAsync(id, cancellationToken), PoliticasTablero.AdministrarTablero);
            if (acceso is not null)
            {
                return acceso;
            }

            return Responder(await _listaTareaService.ReordenarAsync(id, dto, cancellationToken));
        }

        [HttpDelete("listas/{id:int}")]
        public async Task<ActionResult> Eliminar(int id, CancellationToken cancellationToken)
        {
            var acceso = await ValidarAccesoAsync(await _listaTareaService.ObtenerTableroIdAsync(id, cancellationToken), PoliticasTablero.AdministrarTablero);
            if (acceso is not null)
            {
                return acceso;
            }

            return Responder(await _listaTareaService.EliminarAsync(id, cancellationToken));
        }
    }
}
