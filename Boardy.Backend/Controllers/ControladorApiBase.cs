using System.Security.Claims;
using Boardy.Backend.Comun;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boardy.Backend.Controllers
{
    /// <summary>
    /// Comportamiento común de los controladores autenticados: usuario actual, evaluación de
    /// políticas por tablero/tarea y traducción de <see cref="ResultadoOperacion"/> a respuestas HTTP.
    /// </summary>
    [ApiController]
    [Authorize]
    public abstract class ControladorApiBase : ControllerBase
    {
        private readonly IAuthorizationService _autorizacion;

        protected ControladorApiBase(IAuthorizationService autorizacion)
        {
            _autorizacion = autorizacion;
        }

        protected string UsuarioId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        /// <summary>Evalúa una política sobre un recurso (TableroId o TareaId).</summary>
        protected async Task<bool> TienePermisoAsync(int recursoId, string politica)
        {
            var resultado = await _autorizacion.AuthorizeAsync(User, recursoId, politica);
            return resultado.Succeeded;
        }

        /// <summary>404 si el recurso no existe, 403 sin permiso y null si puede continuar.</summary>
        protected async Task<ActionResult?> ValidarAccesoAsync(int? recursoId, string politica)
        {
            if (recursoId is null)
            {
                return NotFound();
            }

            return await TienePermisoAsync(recursoId.Value, politica) ? null : Forbid();
        }

        /// <summary>200 con los datos del resultado, o el error correspondiente.</summary>
        protected ActionResult<T> Responder<T>(ResultadoOperacion<T> resultado) =>
            resultado.Exito ? Ok(resultado.Datos) : Error(resultado);

        /// <summary>204 si la operación salió bien, o el error correspondiente.</summary>
        protected ActionResult Responder(ResultadoOperacion resultado) =>
            resultado.Exito ? NoContent() : Error(resultado);

        private ActionResult Error(ResultadoOperacion resultado)
        {
            if (resultado.NoEncontrado)
            {
                return Problem(resultado.Mensaje, statusCode: StatusCodes.Status404NotFound);
            }

            if (resultado.SinPermiso)
            {
                return Problem(resultado.Mensaje, statusCode: StatusCodes.Status403Forbidden);
            }

            if (resultado.Errores.Count > 0)
            {
                foreach (var (campo, mensaje) in resultado.Errores)
                {
                    ModelState.AddModelError(campo, mensaje);
                }

                return ValidationProblem(ModelState);
            }

            return Problem(resultado.Mensaje, statusCode: StatusCodes.Status400BadRequest);
        }
    }
}
