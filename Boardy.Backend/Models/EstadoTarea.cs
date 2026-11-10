namespace Boardy.Backend.Models
{
    public class EstadoTarea
    {
        public int EstadoTareaId { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        /// <summary>Estado creado con el tablero (Pendiente, En progreso, Finalizada): no se edita ni elimina.</summary>
        public bool EsBase { get; set; }

        public int TableroId { get; set; }

        public Tablero Tablero { get; set; } = null!;

        public ICollection<Tarea> Tareas { get; set; } = new List<Tarea>();

        public ICollection<ListaTarea> Listas { get; set; } = new List<ListaTarea>();
    }
}
