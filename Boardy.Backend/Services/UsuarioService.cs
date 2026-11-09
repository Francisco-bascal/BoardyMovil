using Boardy.Backend.Autorizacion;
using Boardy.Backend.Comun;
using Boardy.Backend.Data;
using Boardy.Backend.DTOs;
using Boardy.Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Boardy.Backend.Services
{
    public class UsuarioService : IUsuarioService
    {
        private const int MaximoResultadosBusqueda = 20;

        private readonly UserManager<Usuario> _userManager;
        private readonly AppDbContext _context;
        private readonly IFotoPerfilService _fotoPerfilService;

        public UsuarioService(UserManager<Usuario> userManager, AppDbContext context, IFotoPerfilService fotoPerfilService)
        {
            _userManager = userManager;
            _context = context;
            _fotoPerfilService = fotoPerfilService;
        }

        public async Task<IReadOnlyList<UsuarioAdministracionDto>> ListarAsync(CancellationToken cancellationToken = default)
        {
            var administradores = (await _userManager.GetUsersInRoleAsync(RolesGlobales.Administrador))
                .Select(u => u.Id)
                .ToHashSet();

            var usuarios = await _userManager.Users
                .AsNoTracking()
                .Where(u => u.Email != null) // las cuentas eliminadas quedan anonimizadas, sin correo
                .OrderBy(u => u.Nombre)
                .ThenBy(u => u.Apellido)
                .Select(u => new UsuarioAdministracionDto
                {
                    UsuarioId = u.Id,
                    Nombre = u.Nombre,
                    Apellido = u.Apellido,
                    NombreCompleto = u.Nombre + " " + u.Apellido,
                    Email = u.Email ?? string.Empty,
                    FotoPerfil = u.FotoPerfil,
                    FechaRegistro = u.FechaRegistro,
                    Activo = u.Estado
                })
                .ToListAsync(cancellationToken);

            usuarios.ForEach(u => u.EsAdministrador = administradores.Contains(u.UsuarioId));
            return usuarios;
        }

        public async Task<ResultadoOperacion<UsuarioAdministracionDto>> ObtenerAsync(string usuarioId, CancellationToken cancellationToken = default)
        {
            var usuario = await _userManager.FindByIdAsync(usuarioId);

            return usuario is null || usuario.Email is null
                ? ResultadoOperacion<UsuarioAdministracionDto>.NoExiste("El usuario no existe.")
                : ResultadoOperacion<UsuarioAdministracionDto>.Exitoso(await MapearAdministracionAsync(usuario));
        }

        public async Task<IReadOnlyList<UsuarioBusquedaDto>> BuscarAsync(string texto, CancellationToken cancellationToken = default)
        {
            var filtro = texto.Trim();

            return await _userManager.Users
                .AsNoTracking()
                .Where(u => u.Estado && u.Email != null
                    && (u.Nombre.Contains(filtro) || u.Apellido.Contains(filtro) || u.Email.Contains(filtro)))
                .OrderBy(u => u.Nombre)
                .ThenBy(u => u.Apellido)
                .Take(MaximoResultadosBusqueda)
                .Select(u => new UsuarioBusquedaDto
                {
                    UsuarioId = u.Id,
                    NombreCompleto = u.Nombre + " " + u.Apellido,
                    Email = u.Email!,
                    FotoPerfil = u.FotoPerfil
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<ResultadoOperacion<UsuarioAdministracionDto>> CrearAsync(RegistrarUsuarioDto dto)
        {
            var email = dto.Email.Trim();

            // La app móvil no envía correos: la cuenta nace con el correo confirmado.
            var usuario = new Usuario
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                Nombre = dto.Nombre.Trim(),
                Apellido = dto.Apellido.Trim(),
                FechaRegistro = DateTime.UtcNow,
                Estado = true
            };

            var resultado = await _userManager.CreateAsync(usuario, dto.Contrasena);

            return resultado.Succeeded
                ? ResultadoOperacion<UsuarioAdministracionDto>.Exitoso(await MapearAdministracionAsync(usuario), "Usuario creado correctamente.")
                : ResultadoOperacion<UsuarioAdministracionDto>.Fallo(Errores(resultado));
        }

        public async Task<ResultadoOperacion<UsuarioAdministracionDto>> EditarAsync(string usuarioId, EditarUsuarioDto dto)
        {
            var usuario = await _userManager.FindByIdAsync(usuarioId);
            if (usuario is null || usuario.Email is null)
            {
                return ResultadoOperacion<UsuarioAdministracionDto>.NoExiste("El usuario no existe.");
            }

            var cambiaContrasena = !string.IsNullOrWhiteSpace(dto.NuevaContrasena);

            // La contraseña se valida antes de tocar nada, para no guardar los datos a medias.
            if (cambiaContrasena)
            {
                foreach (var validador in _userManager.PasswordValidators)
                {
                    var validacion = await validador.ValidateAsync(_userManager, usuario, dto.NuevaContrasena);
                    if (!validacion.Succeeded)
                    {
                        return ResultadoOperacion<UsuarioAdministracionDto>.Fallo(Errores(validacion));
                    }
                }
            }

            AplicarDatosPersonales(usuario, dto.Nombre, dto.Apellido, dto.Email);

            var resultado = await _userManager.UpdateAsync(usuario);
            if (!resultado.Succeeded)
            {
                return ResultadoOperacion<UsuarioAdministracionDto>.Fallo(Errores(resultado));
            }

            if (cambiaContrasena)
            {
                // El token de restablecimiento valida y reemplaza la contraseña en un solo paso.
                var token = await _userManager.GeneratePasswordResetTokenAsync(usuario);
                resultado = await _userManager.ResetPasswordAsync(usuario, token, dto.NuevaContrasena);

                if (!resultado.Succeeded)
                {
                    return ResultadoOperacion<UsuarioAdministracionDto>.Fallo(Errores(resultado));
                }
            }

            return ResultadoOperacion<UsuarioAdministracionDto>.Exitoso(await MapearAdministracionAsync(usuario), "Usuario actualizado correctamente.");
        }

        public async Task<ResultadoOperacion> AlternarEstadoAsync(string usuarioId, string usuarioActualId)
        {
            var (usuario, error) = await ObtenerOtroUsuarioAsync(usuarioId, usuarioActualId);
            if (error is not null)
            {
                return error;
            }

            usuario!.Estado = !usuario.Estado;

            // Cambiar el sello guarda al usuario e invalida sus tokens emitidos.
            var resultado = await _userManager.UpdateSecurityStampAsync(usuario);

            return resultado.Succeeded
                ? ResultadoOperacion.Exitoso(usuario.Estado ? "Usuario activado." : "Usuario desactivado.")
                : ResultadoOperacion.Fallo(Errores(resultado));
        }

        public async Task<ResultadoOperacion> AlternarAdministradorAsync(string usuarioId, string usuarioActualId)
        {
            var (usuario, error) = await ObtenerOtroUsuarioAsync(usuarioId, usuarioActualId);
            if (error is not null)
            {
                return error;
            }

            var eraAdministrador = await _userManager.IsInRoleAsync(usuario!, RolesGlobales.Administrador);

            var resultado = eraAdministrador
                ? await _userManager.RemoveFromRoleAsync(usuario!, RolesGlobales.Administrador)
                : await _userManager.AddToRoleAsync(usuario!, RolesGlobales.Administrador);

            if (!resultado.Succeeded)
            {
                return ResultadoOperacion.Fallo(Errores(resultado));
            }

            await _userManager.UpdateSecurityStampAsync(usuario!);

            return ResultadoOperacion.Exitoso(eraAdministrador ? "Se quitó el rol Administrador." : "Se asignó el rol Administrador.");
        }

        public async Task<ResultadoOperacion<PerfilDto>> ObtenerPerfilAsync(string usuarioId)
        {
            var usuario = await _userManager.FindByIdAsync(usuarioId);

            return usuario is null
                ? ResultadoOperacion<PerfilDto>.NoExiste("El usuario no existe.")
                : ResultadoOperacion<PerfilDto>.Exitoso(await MapearPerfilAsync(usuario));
        }

        public async Task<ResultadoOperacion<PerfilDto>> ActualizarPerfilAsync(string usuarioId, ActualizarPerfilDto dto)
        {
            var usuario = await _userManager.FindByIdAsync(usuarioId);
            if (usuario is null)
            {
                return ResultadoOperacion<PerfilDto>.NoExiste("El usuario no existe.");
            }

            AplicarDatosPersonales(usuario, dto.Nombre, dto.Apellido, dto.Email);
            var resultado = await _userManager.UpdateAsync(usuario);

            return resultado.Succeeded
                ? ResultadoOperacion<PerfilDto>.Exitoso(await MapearPerfilAsync(usuario), "Perfil actualizado correctamente.")
                : ResultadoOperacion<PerfilDto>.Fallo(Errores(resultado));
        }

        public async Task<ResultadoOperacion> CambiarContrasenaAsync(string usuarioId, CambiarContrasenaDto dto)
        {
            var usuario = await _userManager.FindByIdAsync(usuarioId);
            if (usuario is null)
            {
                return ResultadoOperacion.NoExiste("El usuario no existe.");
            }

            var resultado = await _userManager.ChangePasswordAsync(usuario, dto.ContrasenaActual, dto.ContrasenaNueva);

            return resultado.Succeeded
                ? ResultadoOperacion.Exitoso("Contraseña actualizada correctamente.")
                : ResultadoOperacion.Fallo(Errores(resultado));
        }

        public async Task<ResultadoOperacion> EliminarCuentaAsync(string usuarioId, CancellationToken cancellationToken = default)
        {
            var usuario = await _userManager.FindByIdAsync(usuarioId);
            if (usuario is null)
            {
                return ResultadoOperacion.NoExiste("El usuario no existe.");
            }

            // No eliminación de propietario: primero debe transferir o eliminar sus tableros.
            if (await _context.Tableros.AnyAsync(t => t.PropietarioId == usuario.Id, cancellationToken))
            {
                return ResultadoOperacion.Fallo("Sos propietario de al menos un tablero: transferí la propiedad o eliminalo antes de borrar tu cuenta.");
            }

            var esAdministrador = await _userManager.IsInRoleAsync(usuario, RolesGlobales.Administrador);
            // Regla de no eliminación de único Administrador: la plataforma debe conservar uno.
            if (esAdministrador && (await _userManager.GetUsersInRoleAsync(RolesGlobales.Administrador)).Count == 1)
            {
                return ResultadoOperacion.Fallo("Sos el único administrador de la plataforma: asigná otro antes de borrar tu cuenta.");
            }

            if (usuario.FotoPerfil is not null)
            {
                await _fotoPerfilService.QuitarAsync(usuario);
            }

            _context.TareaMiembros.RemoveRange(await _context.TareaMiembros.Where(m => m.TableroMiembro.UsuarioId == usuario.Id).ToListAsync(cancellationToken));
            _context.TableroMiembros.RemoveRange(await _context.TableroMiembros.Where(m => m.UsuarioId == usuario.Id).ToListAsync(cancellationToken));
            _context.InvitacionTableros.RemoveRange(await _context.InvitacionTableros
                .Where(i => i.UsuarioInvitadoId == usuario.Id || i.UsuarioInvitadorId == usuario.Id)
                .ToListAsync(cancellationToken));

            if (esAdministrador)
            {
                await _userManager.RemoveFromRoleAsync(usuario, RolesGlobales.Administrador);
            }

            // Regla de anonimización: se conservan comentarios y auditoría como "Usuario eliminado".
            usuario.Nombre = "Usuario";
            usuario.Apellido = "eliminado";
            usuario.UserName = $"eliminado-{Guid.NewGuid():N}";
            usuario.Email = null;
            usuario.EmailConfirmed = false;
            usuario.PhoneNumber = null;
            usuario.PasswordHash = null;
            usuario.Estado = false;

            // Cambiar el sello guarda todo lo anterior e invalida los tokens emitidos.
            var resultado = await _userManager.UpdateSecurityStampAsync(usuario);

            return resultado.Succeeded
                ? ResultadoOperacion.Exitoso("Tu cuenta fue eliminada.")
                : ResultadoOperacion.Fallo(Errores(resultado));
        }

        private async Task<(Usuario? Usuario, ResultadoOperacion? Error)> ObtenerOtroUsuarioAsync(string usuarioId, string usuarioActualId)
        {
            // No modificación de usuario propio: no podés desactivarte ni cambiar tu rol global.
            if (usuarioId == usuarioActualId)
            {
                return (null, ResultadoOperacion.Fallo("No podés aplicar esta acción sobre tu propia cuenta."));
            }

            var usuario = await _userManager.FindByIdAsync(usuarioId);

            return usuario is null
                ? (null, ResultadoOperacion.NoExiste("El usuario no existe."))
                : (usuario, null);
        }

        // El correo es también el nombre de usuario, como en la versión web.
        private static void AplicarDatosPersonales(Usuario usuario, string nombre, string apellido, string email)
        {
            usuario.Nombre = nombre.Trim();
            usuario.Apellido = apellido.Trim();
            usuario.Email = email.Trim();
            usuario.UserName = email.Trim();
        }

        private async Task<UsuarioAdministracionDto> MapearAdministracionAsync(Usuario usuario) => new()
        {
            UsuarioId = usuario.Id,
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            NombreCompleto = usuario.Nombre + " " + usuario.Apellido,
            Email = usuario.Email ?? string.Empty,
            FotoPerfil = usuario.FotoPerfil,
            FechaRegistro = usuario.FechaRegistro,
            Activo = usuario.Estado,
            EsAdministrador = await _userManager.IsInRoleAsync(usuario, RolesGlobales.Administrador)
        };

        private async Task<PerfilDto> MapearPerfilAsync(Usuario usuario) => new()
        {
            UsuarioId = usuario.Id,
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            Email = usuario.Email ?? string.Empty,
            FotoPerfil = usuario.FotoPerfil,
            EsAdministrador = await _userManager.IsInRoleAsync(usuario, RolesGlobales.Administrador)
        };

        private static string Errores(IdentityResult resultado) =>
            string.Join(" ", resultado.Errors.Select(e => e.Description));
    }
}
