using Boardy.Backend.DTOs;
using Boardy.Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boardy.Backend.Controllers
{
    [Route("api/autenticacion")]
    [AllowAnonymous]
    public class AutenticacionController : ControladorApiBase
    {
        private readonly IAutenticacionService _autenticacionService;

        public AutenticacionController(IAutenticacionService autenticacionService, IAuthorizationService autorizacion)
            : base(autorizacion)
        {
            _autenticacionService = autenticacionService;
        }

        [HttpPost("registro")]
        public async Task<ActionResult<SesionDto>> Registrar(RegistrarUsuarioDto dto) =>
            Responder(await _autenticacionService.RegistrarAsync(dto));

        [HttpPost("inicio-sesion")]
        public async Task<ActionResult<SesionDto>> IniciarSesion(IniciarSesionDto dto) =>
            Responder(await _autenticacionService.IniciarSesionAsync(dto));
    }
}
