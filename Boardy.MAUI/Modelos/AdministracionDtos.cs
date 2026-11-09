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

        /// <summary>Si viene vacía, la contraseña no cambia.</summary>
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre {2} y {1} caracteres.")]
        public string? NuevaContrasena { get; set; }
    }
}
