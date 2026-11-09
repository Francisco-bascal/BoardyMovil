namespace Boardy.MAUI
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

#if ANDROID
            // La WebView se sirve por https://0.0.0.1: sin esto bloquea las fotos de perfil del backend HTTP de desarrollo.
            blazorWebView.BlazorWebViewInitialized += (_, e) =>
                e.WebView.Settings.MixedContentMode = Android.Webkit.MixedContentHandling.AlwaysAllow;
#endif
        }
    }
}
