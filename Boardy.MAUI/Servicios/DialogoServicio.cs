namespace Boardy.MAUI.Servicios
{
    /// <summary>Diálogos nativos de la plataforma (más fiables que window.confirm dentro de la WebView).</summary>
    public class DialogoServicio
    {
        public Task<bool> ConfirmarAsync(string mensaje, string aceptar = "Sí") =>
            MainThread.InvokeOnMainThreadAsync(() =>
                Application.Current!.Windows[0].Page!.DisplayAlert("Boardy", mensaje, aceptar, "Cancelar"));
    }
}
