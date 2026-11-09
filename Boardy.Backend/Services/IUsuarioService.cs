using Boardy.Backend.Comun;
using Boardy.Backend.DTOs;

namespace Boardy.Backend.Services
{
    public interface IUsuarioService
    {
        Task<IReadOnlyList<UsuarioAdministracionDto>> ListarAsync(CancellationToken cancellationToken = default);

        Task<ResultadoOperacion<UsuarioAdministracionDto>> ObtenerAsync(string usuarioId, CancellationToken cancellationToken = default);

        /// <summary>Usuarios activos cuyo nombre, apellido o correo contienen el texto. Se usa para invitar a un tablero.</summary>
        Task<IReadOnlyList<UsuarioBusquedaDto>> BuscarAsync(string texto, CancellationToken cancellationToken = default);

        /// <summary>Crea una cuenta activa y con el correo confirmado (registro propio o alta por un administrador).</summary>
        Task<ResultadoOperacion<UsuarioAdministracionDto>> CrearAsync(RegistrarUsuarioDto dto);

        /// <summary>Edición de un usuario por un administrador.</summary>
        Task<ResultadoOperacion<UsuarioAdministracionDto>> EditarAsync(string usuarioId, EditarUsuarioDto dto);

        /// <summary>Activa o desactiva la cuenta. Nadie puede aplicárselo a sí mismo.</summary>
        Task<ResultadoOperacion> AlternarEstadoAsync(string usuarioId, string usuarioActualId);

        /// <summary>Da o quita el rol global Administrador. Nadie puede aplicárselo a sí mismo.</summary>
        Task<ResultadoOperacion> AlternarAdministradorAsync(string usuarioId, string usuarioActualId);

        Task<ResultadoOperacion<PerfilDto>> ObtenerPerfilAsync(string usuarioId);

        Task<ResultadoOperacion<PerfilDto>> ActualizarPerfilAsync(string usuarioId, ActualizarPerfilDto dto);

        Task<ResultadoOperacion> CambiarContrasenaAsync(string usuarioId, CambiarContrasenaDto dto);

        /// <summary>
        /// Elimina la cuenta propia: quita membresías e invitaciones y anonimiza al usuario, de modo que
        /// sus comentarios y su auditoría se conservan. No se permite a propietarios de tableros
        /// ni al único administrador global.
        /// </summary>
        Task<ResultadoOperacion> EliminarCuentaAsync(string usuarioId, CancellationToken cancellationToken = default);
    }
}
