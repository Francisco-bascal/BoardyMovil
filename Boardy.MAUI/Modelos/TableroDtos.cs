using System.ComponentModel.DataAnnotations;

namespace Boardy.MAUI.Modelos
{
    public class TableroDto
    {
        public int TableroId { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        public DateTime FechaCreacion { get; set; }
    }

    public class TableroDetalleDto
    {
        public int TableroId { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        public string PropietarioNombre { get; set; } = string.Empty;

        public bool EsPropietario { get; set; }

        public bool EsKanban { get; set; }

        /// <summary>Rol del usuario actual en el tablero; la app lo usa para mostrar u ocultar acciones.</summary>
        public string RolUsuario { get; set; } = string.Empty;
    }

    public class CrearTableroDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(150, ErrorMessage = "El nombre no puede superar los {1} caracteres.")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "La descripción no puede superar los {1} caracteres.")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; } = string.Empty;

        // Solo se usa al crear: el esquema no cambia después.
        [Display(Name = "Esquema")]
        public bool EsKanban { get; set; } = true;
    }

    public class ActualizarTableroDto : CrearTableroDto
    {
    }
}
