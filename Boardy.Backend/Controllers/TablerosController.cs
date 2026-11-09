using Boardy.Backend.Autorizacion;
using Boardy.Backend.DTOs;
using Boardy.Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boardy.Backend.Controllers
{
    [Route("api/tableros")]
    public class TablerosController : ControladorApiBase
    {
        private readonly ITableroService _tableroService;

        public TablerosController(ITableroService tableroService, IAuthorizationService autorizacion)
            : base(autorizacion)
        {
            _tableroService = tableroService;
        }

        /// <summary>Tableros de los que el usuario es miembro.</summary>
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<TableroDto>>> Listar(CancellationToken cancellationToken) =>
            Ok(await _tableroService.ListarDelUsuarioAsync(UsuarioId, cancellationToken));

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TableroDetalleDto>> Obtener(int id, CancellationToken cancellationToken)
        {
            var acceso = await ValidarAccesoAsync(id, PoliticasTablero.VerTablero);
            if (acceso is not null)
            {
                return acceso;
            }

            return Responder(await _tableroService.ObtenerDetalleAsync(id, UsuarioId, cancellationToken));
        }

        [HttpPost]
        public async Task<ActionResult<TableroDto>> Crear(CrearTableroDto dto, CancellationToken cancellationToken)
        {
            var resultado = await _tableroService.CrearAsync(dto, UsuarioId, cancellationToken);

            return resultado.Exito
                ? CreatedAtAction(nameof(Obtener), new { id = resultado.Datos!.TableroId }, resultado.Datos)
                : Responder(resultado);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<TableroDto>> Actualizar(int id, ActualizarTableroDto dto, CancellationToken cancellationToken)
        {
            var acceso = await ValidarAccesoAsync(id, PoliticasTablero.AdministrarTablero);
            if (acceso is not null)
            {
                return acceso;
            }

            return Responder(await _tableroService.ActualizarAsync(id, dto, cancellationToken));
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Eliminar(int id, CancellationToken cancellationToken)
        {
            var acceso = await ValidarAccesoAsync(id, PoliticasTablero.GestionarTablero);
            if (acceso is not null)
            {
                return acceso;
            }

            return Responder(await _tableroService.EliminarAsync(id, cancellationToken));
        }
    }
}
