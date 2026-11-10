using System.ComponentModel.DataAnnotations;

namespace Boardy.MAUI.Modelos
{
    public class UsuarioAdministracionDto
    {
        public string UsuarioId { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;

        public string Apellido { get; set; } = string.Empty;

        public string NombreCompleto { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? FotoPerfil { get; set; }

        public DateTime FechaRegistro { get; set; }

        public bool Activo { get; set; }

        public bool EsAdministrador { get; set; }
    }

    public class EditarUsuarioDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre no puede superar los {1} caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(50, ErrorMessage = "El apellido no puede superar los {1} caracteres.")]
        public string Apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido")]
        public string Email { get; set; } = string.Empty;

        /// <summary>Si viene vacía, la contraseña no cambia (RegularExpression, a diferencia de StringLength, acepta el vacío).</summary>
        [RegularExpression("^.{6,100}$", ErrorMessage = "La contraseña debe tener entre 6 y 100 caracteres.")]
        public string? NuevaContrasena { get; set; }
    }

    public class RegistroErrorDto
    {
        public string Mensaje { get; set; } = string.Empty;

        public string Detalle { get; set; } = string.Empty;

        public string Ruta { get; set; } = string.Empty;

        public string? UsuarioId { get; set; }

        public DateTime Fecha { get; set; }
    }

    public class RegistroAuditoriaDto
    {
        public string UsuarioNombre { get; set; } = string.Empty;

        public string Accion { get; set; } = string.Empty;

        public DateTime Fecha { get; set; }
    }
}
