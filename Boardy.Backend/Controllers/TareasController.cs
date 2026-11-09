using Boardy.Backend.Autorizacion;
using Boardy.Backend.DTOs;
using Boardy.Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boardy.Backend.Controllers
{
    [Route("api")]
    public class TareasController : ControladorApiBase
    {
        private readonly ITareaService _tareaService;
        private readonly IListaTareaService _listaTareaService;
        private readonly IEtiquetaService _etiquetaService;

        public TareasController(
            ITareaService tareaService,
            IListaTareaService listaTareaService,
            IEtiquetaService etiquetaService,
            IAuthorizationService autorizacion)
            : base(autorizacion)
        {
            _tareaService = tareaService;
            _listaTareaService = listaTareaService;
            _etiquetaService = etiquetaService;
        }

        [HttpGet("tableros/{tableroId:int}/tareas")]
        public async Task<ActionResult<IReadOnlyList<TareaDto>>> ListarPorTablero(int tableroId, CancellationToken cancellationToken)
        {
            var acceso = await ValidarAccesoAsync(tableroId, PoliticasTablero.VerTablero);
            if (acceso is not null)
            {
                return acceso;
            }

            return Ok(await _tareaService.ListarPorTableroAsync(tableroId, cancellationToken));
        }

        [HttpGet("tareas/{id:int}")]
        public async Task<ActionResult<TareaDto>> Obtener(int id, CancellationToken cancellationToken)
        {
            var acceso = await ValidarAccesoATareaAsync(id, PoliticasTablero.VerTablero, cancellationToken);
            if (acceso is not null)
            {
                return acceso;
            }

            return Responder(await _tareaService.ObtenerAsync(id, cancellationToken));
        }

        [HttpPost("listas/{listaTareaId:int}/tareas")]
        public async Task<ActionResult<TareaDto>> Crear(int listaTareaId, CrearTareaDto dto, CancellationToken cancellationToken)
        {
            var acceso = await ValidarAccesoAsync(await _listaTareaService.ObtenerTableroIdAsync(listaTareaId, cancellationToken), PoliticasTablero.AdministrarTablero);
            if (acceso is not null)
            {
                return acceso;
            }

            var resultado = await _tareaService.CrearAsync(listaTareaId, dto, cancellationToken);

            return resultado.Exito
                ? CreatedAtAction(nameof(Obtener), new { id = resultado.Datos!.TareaId }, resultado.Datos)
                : Responder(resultado);
        }

        [HttpPut("tareas/{id:int}")]
        public async Task<ActionResult<TareaDto>> Actualizar(int id, ActualizarTareaDto dto, CancellationToken cancellationToken)
        {
            var acceso = await ValidarAccesoATareaAsync(id, PoliticasTablero.AdministrarTablero, cancellationToken);
            if (acceso is not null)
            {
                return acceso;
            }

            return Responder(await _tareaService.ActualizarAsync(id, dto, cancellationToken));
        }

        [HttpDelete("tareas/{id:int}")]
        public async Task<ActionResult> Eliminar(int id, CancellationToken cancellationToken)
        {
            var acceso = await ValidarAccesoATareaAsync(id, PoliticasTablero.AdministrarTablero, cancellationToken);
            if (acceso is not null)
            {
                return acceso;
            }

            return Responder(await _tareaService.EliminarAsync(id, cancellationToken));
        }

        [HttpPut("tareas/{id:int}/mover")]
        public async Task<ActionResult<TareaDto>> Mover(int id, MoverTareaDto dto, CancellationToken cancellationToken)
        {
            var acceso = await ValidarTrabajoEnTareaAsync(id, cancellationToken);
            if (acceso is not null)
            {
                return acceso;
            }

            return Responder(await _tareaService.MoverAsync(id, dto, cancellationToken));
        }

        [HttpPut("tareas/{id:int}/estado")]
        public async Task<ActionResult<TareaDto>> CambiarEstado(int id, CambiarEstadoTareaDto dto, CancellationToken cancellationToken)
        {
            var acceso = await ValidarTrabajoEnTareaAsync(id, cancellationToken);
            if (acceso is not null)
            {
                return acceso;
            }

            return Responder(await _tareaService.CambiarEstadoAsync(id, dto, cancellationToken));
        }

        [HttpPost("tareas/{id:int}/etiquetas/{etiquetaId:int}")]
        public async Task<ActionResult> AsignarEtiqueta(int id, int etiquetaId, CancellationToken cancellationToken)
        {
            var acceso = await ValidarTrabajoEnTareaAsync(id, cancellationToken);
            if (acceso is not null)
            {
                return acceso;
            }

            return Responder(await _etiquetaService.AsignarATareaAsync(id, etiquetaId, cancellationToken));
        }

        [HttpDelete("tareas/{id:int}/etiquetas/{etiquetaId:int}")]
        public async Task<ActionResult> QuitarEtiqueta(int id, int etiquetaId, CancellationToken cancellationToken)
        {
            var acceso = await ValidarTrabajoEnTareaAsync(id, cancellationToken);
            if (acceso is not null)
            {
                return acceso;
            }

            return Responder(await _etiquetaService.QuitarDeTareaAsync(id, etiquetaId, cancellationToken));
        }

        [HttpPost("tareas/{id:int}/miembros/{tableroMiembroId:int}")]
        public async Task<ActionResult<TareaDto>> AsignarMiembro(int id, int tableroMiembroId, CancellationToken cancellationToken)
        {
            var acceso = await ValidarAccesoATareaAsync(id, PoliticasTablero.AdministrarTablero, cancellationToken);
            if (acceso is not null)
            {
                return acceso;
            }

            return Responder(await _tareaService.AsignarMiembroAsync(id, tableroMiembroId, cancellationToken));
        }

        [HttpDelete("tareas/{id:int}/miembros/{tableroMiembroId:int}")]
        public async Task<ActionResult<TareaDto>> QuitarMiembro(int id, int tableroMiembroId, CancellationToken cancellationToken)
        {
            var acceso = await ValidarAccesoATareaAsync(id, PoliticasTablero.AdministrarTablero, cancellationToken);
            if (acceso is not null)
            {
                return acceso;
            }

            return Responder(await _tareaService.QuitarMiembroAsync(id, tableroMiembroId, cancellationToken));
        }

        private async Task<ActionResult?> ValidarAccesoATareaAsync(int tareaId, string politicaTablero, CancellationToken cancellationToken) =>
            await ValidarAccesoAsync(await _tareaService.ObtenerTableroIdAsync(tareaId, cancellationToken), politicaTablero);

        // Mover, cambiar estado y etiquetar: Propietario y Administrador en cualquier tarea; Operario solo en las asignadas.
        private async Task<ActionResult?> ValidarTrabajoEnTareaAsync(int tareaId, CancellationToken cancellationToken)
        {
            if (await _tareaService.ObtenerTableroIdAsync(tareaId, cancellationToken) is null)
            {
                return NotFound();
            }

            return await TienePermisoAsync(tareaId, PoliticasTarea.TrabajarEnTarea) ? null : Forbid();
        }
    }
}
