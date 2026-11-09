using System.Net.Http.Headers;
using Boardy.MAUI.Servicios;
using Microsoft.Extensions.Logging;

namespace Boardy.MAUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

            builder.Services.AddSingleton<SesionServicio>();
            builder.Services.AddSingleton(new HttpClient
            {
                BaseAddress = new Uri(ConfiguracionApi.UrlBase),
                Timeout = TimeSpan.FromSeconds(20),
                // Sin esto, ASP.NET Core devuelve los string como texto plano en lugar de JSON.
                DefaultRequestHeaders = { Accept = { new MediaTypeWithQualityHeaderValue("application/json") } }
            });
            builder.Services.AddSingleton<ApiCliente>();
            builder.Services.AddSingleton<AutenticacionServicio>();
            builder.Services.AddSingleton<TableroServicio>();
            builder.Services.AddSingleton<TareaServicio>();
            builder.Services.AddSingleton<MiembroServicio>();
            builder.Services.AddSingleton<UsuarioServicio>();
            builder.Services.AddSingleton<DialogoServicio>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
