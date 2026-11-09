using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Boardy.Backend.Comun;
using Boardy.Backend.DTOs;
using Boardy.Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Boardy.Backend.Services
{
    /// <summary>
    /// Registro e inicio de sesión de la app móvil: valida credenciales con Identity y emite un JWT.
    /// </summary>
    public class AutenticacionService : IAutenticacionService
    {
        private const string MensajeCredencialesInvalidas = "El correo o la contraseña son incorrectos.";

        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly IUsuarioService _usuarioService;
        private readonly OpcionesJwt _opcionesJwt;

        public AutenticacionService(
            UserManager<Usuario> userManager,
            SignInManager<Usuario> signInManager,
            IUsuarioService usuarioService,
            IOptions<OpcionesJwt> opcionesJwt)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _usuarioService = usuarioService;
            _opcionesJwt = opcionesJwt.Value;
        }

        public async Task<ResultadoOperacion<SesionDto>> RegistrarAsync(RegistrarUsuarioDto dto)
        {
            var creado = await _usuarioService.CrearAsync(dto);
            if (!creado.Exito)
            {
                return ResultadoOperacion<SesionDto>.Fallo(creado.Mensaje!);
            }

            var usuario = await _userManager.FindByIdAsync(creado.Datos!.UsuarioId);
            return ResultadoOperacion<SesionDto>.Exitoso(await CrearSesionAsync(usuario!));
        }

        public async Task<ResultadoOperacion<SesionDto>> IniciarSesionAsync(IniciarSesionDto dto)
        {
            var usuario = await _userManager.FindByEmailAsync(dto.Email.Trim());

            // Mismo mensaje para correo inexistente y contraseña incorrecta: no revela qué cuentas existen.
            if (usuario is null)
            {
                return ResultadoOperacion<SesionDto>.Fallo(MensajeCredencialesInvalidas);
            }

            var resultado = await _signInManager.CheckPasswordSignInAsync(usuario, dto.Contrasena, lockoutOnFailure: true);

            if (resultado.IsLockedOut)
            {
                return ResultadoOperacion<SesionDto>.Fallo("La cuenta está bloqueada temporalmente por intentos fallidos. Probá más tarde.");
            }

            if (!resultado.Succeeded)
            {
                return ResultadoOperacion<SesionDto>.Fallo(MensajeCredencialesInvalidas);
            }

            if (!usuario.Estado)
            {
                return ResultadoOperacion<SesionDto>.Fallo("La cuenta está desactivada.");
            }

            return ResultadoOperacion<SesionDto>.Exitoso(await CrearSesionAsync(usuario));
        }

        private async Task<SesionDto> CrearSesionAsync(Usuario usuario)
        {
            var roles = await _userManager.GetRolesAsync(usuario);
            var expiracion = DateTime.UtcNow.AddHours(_opcionesJwt.DuracionHoras);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, usuario.Id),
                new(ClaimTypes.Email, usuario.Email!),
                new(ClaimTypes.Name, usuario.Nombre + " " + usuario.Apellido),
                // El sello permite invalidar el token al desactivar al usuario o cambiar su rol.
                new(OpcionesJwt.ClaimSelloSeguridad, await _userManager.GetSecurityStampAsync(usuario))
            };
            claims.AddRange(roles.Select(rol => new Claim(ClaimTypes.Role, rol)));

            var credenciales = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opcionesJwt.Clave)),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _opcionesJwt.Emisor,
                audience: _opcionesJwt.Audiencia,
                claims: claims,
                expires: expiracion,
                signingCredentials: credenciales);

            return new SesionDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiracion = expiracion,
                Usuario = (await _usuarioService.ObtenerPerfilAsync(usuario.Id)).Datos!
            };
        }
    }
}
