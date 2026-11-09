namespace Boardy.MAUI.Servicios
{
    /// <summary>
    /// Qué acciones mostrar según el rol del usuario en el tablero. Solo decide la interfaz:
    /// el backend vuelve a validar cada operación con sus políticas.
    /// </summary>
    public static class PermisosTablero
    {
        public const string Propietario = "Propietario";
        public const string Administrador = "Administrador";
        public const string Operario = "Operario";

        public static bool PuedeAdministrar(string rol) => rol is Propietario or Administrador;

        public static bool PuedeColaborar(string rol) => rol is Propietario or Administrador or Operario;

        public static bool PuedeGestionar(string rol) => rol == Propietario;
    }
}
