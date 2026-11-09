using System.ComponentModel.DataAnnotations;

namespace Boardy.MAUI.Modelos
{
    public class PerfilDto
    {
        public string UsuarioId { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;

        public string Apellido { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? FotoPerfil { get; set; }

        public bool EsAdministrador { get; set; }
    }

    public class ActualizarPerfilDto
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
    }

    public class CambiarContrasenaDto
    {
        [Required(ErrorMessage = "La contraseña actual es obligatoria")]
        public string ContrasenaActual { get; set; } = string.Empty;

        [Required(ErrorMessage = "La nueva contraseña es obligatoria")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre {2} y {1} caracteres.")]
        public string ContrasenaNueva { get; set; } = string.Empty;
    }

    public class UsuarioBusquedaDto
    {
        public string UsuarioId { get; set; } = string.Empty;

        public string NombreCompleto { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? FotoPerfil { get; set; }
    }
}
