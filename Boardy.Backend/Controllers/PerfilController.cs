using Boardy.Backend.DTOs;
using Boardy.Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boardy.Backend.Controllers
{
    /// <summary>Datos de la cuenta del usuario autenticado.</summary>
    [Route("api/perfil")]
    public class PerfilController : ControladorApiBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IFotoPerfilService _fotoPerfilService;

        public PerfilController(IUsuarioService usuarioService, IFotoPerfilService fotoPerfilService, IAuthorizationService autorizacion)
            : base(autorizacion)
        {
            _usuarioService = usuarioService;
            _fotoPerfilService = fotoPerfilService;
        }

        [HttpGet]
        public async Task<ActionResult<PerfilDto>> Obtener() =>
            Responder(await _usuarioService.ObtenerPerfilAsync(UsuarioId));

        [HttpPut]
        public async Task<ActionResult<PerfilDto>> Actualizar(ActualizarPerfilDto dto) =>
            Responder(await _usuarioService.ActualizarPerfilAsync(UsuarioId, dto));

        [HttpPut("contrasena")]
        public async Task<ActionResult> CambiarContrasena(CambiarContrasenaDto dto) =>
            Responder(await _usuarioService.CambiarContrasenaAsync(UsuarioId, dto));

        /// <summary>Sube la foto (multipart, campo "archivo") y devuelve su ruta pública.</summary>
        [HttpPut("foto")]
        public async Task<ActionResult<string>> SubirFoto(IFormFile archivo, CancellationToken cancellationToken) =>
            Responder(await _fotoPerfilService.GuardarAsync(UsuarioId, archivo, cancellationToken));

        [HttpDelete]
        public async Task<ActionResult> EliminarCuenta(CancellationToken cancellationToken) =>
            Responder(await _usuarioService.EliminarCuentaAsync(UsuarioId, cancellationToken));
    }
}
