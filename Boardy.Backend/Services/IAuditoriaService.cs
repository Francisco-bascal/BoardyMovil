namespace Boardy.Backend.Services
{
    public interface IAuditoriaService
    {
        /// <summary>
        /// Agrega el registro al contexto sin guardarlo: se persiste en el SaveChangesAsync de la
        /// operación, de modo que la acción y su registro quedan en la misma transacción.
        /// </summary>
        void Registrar(int tableroId, string accion);
    }
}
