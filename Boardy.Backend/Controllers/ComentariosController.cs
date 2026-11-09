using Boardy.Backend.Autorizacion;
using Boardy.Backend.DTOs;
using Boardy.Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boardy.Backend.Controllers
{
    [Route("api")]
    public class ComentariosController : ControladorApiBase
    {
        private readonly IComentarioService _comentarioService;
        private readonly ITareaService _tareaService;

        public ComentariosController(IComentarioService comentarioService, ITareaService tareaService, IAuthorizationService autorizacion)
            : base(autorizacion)
        {
            _comentarioService = comentarioService;
            _tareaService = tareaService;
        }

        [HttpGet("tareas/{tareaId:int}/comentarios")]
        public async Task<ActionResult<IReadOnlyList<ComentarioDto>>> Listar(int tareaId, CancellationToken cancellationToken)
        {
            var acceso = await ValidarAccesoAsync(await _tareaService.ObtenerTableroIdAsync(tareaId, cancellationToken), PoliticasTablero.VerTablero);
            if (acceso is not null)
            {
                return acceso;
            }

            return Ok(await _comentarioService.ListarPorTareaAsync(tareaId, cancellationToken));
        }

        [HttpPost("tareas/{tareaId:int}/comentarios")]
        public async Task<ActionResult<ComentarioDto>> Crear(int tareaId, CrearComentarioDto dto, CancellationToken cancellationToken)
        {
            var acceso = await ValidarAccesoAsync(await _tareaService.ObtenerTableroIdAsync(tareaId, cancellationToken), PoliticasTablero.ColaborarEnTablero);
            if (acceso is not null)
            {
                return acceso;
            }

            return Responder(await _comentarioService.CrearAsync(tareaId, dto, UsuarioId, cancellationToken));
        }

        /// <summary>El autor borra su comentario; Propietario y Administrador pueden borrar cualquiera.</summary>
        [HttpDelete("comentarios/{id:int}")]
        public async Task<ActionResult> Eliminar(int id, CancellationToken cancellationToken)
        {
            var tableroId = await _comentarioService.ObtenerTableroIdAsync(id, cancellationToken);
            var acceso = await ValidarAccesoAsync(tableroId, PoliticasTablero.ColaborarEnTablero);
            if (acceso is not null)
            {
                return acceso;
            }

            var puedeModerar = await TienePermisoAsync(tableroId!.Value, PoliticasTablero.AdministrarTablero);
            return Responder(await _comentarioService.EliminarAsync(id, UsuarioId, puedeModerar, cancellationToken));
        }
    }
}
