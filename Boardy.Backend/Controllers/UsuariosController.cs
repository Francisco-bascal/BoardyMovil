using Boardy.Backend.Autorizacion;
using Boardy.Backend.DTOs;
using Boardy.Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boardy.Backend.Controllers
{
    /// <summary>Administración de usuarios de la plataforma, más la búsqueda para invitar a tableros.</summary>
    [Route("api/usuarios")]
    public class UsuariosController : ControladorApiBase
    {
        private const int LargoMinimoBusqueda = 2;

        private readonly IUsuarioService _usuarioService;

        public UsuariosController(IUsuarioService usuarioService, IAuthorizationService autorizacion)
            : base(autorizacion)
        {
            _usuarioService = usuarioService;
        }

        /// <summary>Disponible para cualquier usuario autenticado.</summary>
        [HttpGet("buscar")]
        public async Task<ActionResult<IReadOnlyList<UsuarioBusquedaDto>>> Buscar(string texto, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(texto) || texto.Trim().Length < LargoMinimoBusqueda)
            {
                return Ok(Array.Empty<UsuarioBusquedaDto>());
            }

            return Ok(await _usuarioService.BuscarAsync(texto, cancellationToken));
        }

        [HttpGet]
        [Authorize(Roles = RolesGlobales.Administrador)]
        public async Task<ActionResult<IReadOnlyList<UsuarioAdministracionDto>>> Listar(CancellationToken cancellationToken) =>
            Ok(await _usuarioService.ListarAsync(cancellationToken));

        [HttpGet("{id}")]
        [Authorize(Roles = RolesGlobales.Administrador)]
        public async Task<ActionResult<UsuarioAdministracionDto>> Obtener(string id, CancellationToken cancellationToken) =>
            Responder(await _usuarioService.ObtenerAsync(id, cancellationToken));

        [HttpPost]
        [Authorize(Roles = RolesGlobales.Administrador)]
        public async Task<ActionResult<UsuarioAdministracionDto>> Crear(RegistrarUsuarioDto dto)
        {
            var resultado = await _usuarioService.CrearAsync(dto);

            return resultado.Exito
                ? CreatedAtAction(nameof(Obtener), new { id = resultado.Datos!.UsuarioId }, resultado.Datos)
                : Responder(resultado);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = RolesGlobales.Administrador)]
        public async Task<ActionResult<UsuarioAdministracionDto>> Editar(string id, EditarUsuarioDto dto) =>
            Responder(await _usuarioService.EditarAsync(id, dto));

        [HttpPost("{id}/alternar-estado")]
        [Authorize(Roles = RolesGlobales.Administrador)]
        public async Task<ActionResult> AlternarEstado(string id) =>
            Responder(await _usuarioService.AlternarEstadoAsync(id, UsuarioId));

        [HttpPost("{id}/alternar-administrador")]
        [Authorize(Roles = RolesGlobales.Administrador)]
        public async Task<ActionResult> AlternarAdministrador(string id) =>
            Responder(await _usuarioService.AlternarAdministradorAsync(id, UsuarioId));
    }
}
