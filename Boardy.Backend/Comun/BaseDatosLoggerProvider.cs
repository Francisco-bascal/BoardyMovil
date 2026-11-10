using System.Security.Claims;
using System.Threading.Channels;
using Boardy.Backend.Models;

namespace Boardy.Backend.Comun
{
    /// <summary>
    /// Cola en memoria entre los loggers (que no pueden esperar a la BD) y el servicio que guarda los errores.
    /// </summary>
    public class ColaRegistroErrores
    {
        // shortcut: si la app se cae con errores todavía en la cola, esos registros se pierden; alcanza para un log de diagnóstico.
        public Channel<RegistroError> Canal { get; } = Channel.CreateUnbounded<RegistroError>();
    }

    /// <summary>
    /// Proveedor de ILogger que manda a la tabla RegistrosError todo lo de nivel Error o superior,
    /// incluidas las excepciones no controladas que registra el manejador de excepciones de ASP.NET Core.
    /// </summary>
    public class BaseDatosLoggerProvider : ILoggerProvider
    {
        private readonly ColaRegistroErrores _cola;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BaseDatosLoggerProvider(ColaRegistroErrores cola, IHttpContextAccessor httpContextAccessor)
        {
            _cola = cola;
            _httpContextAccessor = httpContextAccessor;
        }

        public ILogger CreateLogger(string categoria) => new BaseDatosLogger(categoria, this);

        public void Dispose()
        {
        }

        private class BaseDatosLogger : ILogger
        {
            private readonly string _categoria;
            private readonly BaseDatosLoggerProvider _proveedor;

            public BaseDatosLogger(string categoria, BaseDatosLoggerProvider proveedor)
            {
                _categoria = categoria;
                _proveedor = proveedor;
            }

            public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

            // Los errores del propio EF Core se excluyen: guardarlos usaría EF Core y podría entrar en bucle.
            public bool IsEnabled(LogLevel nivel) =>
                nivel >= LogLevel.Error && !_categoria.StartsWith("Microsoft.EntityFrameworkCore", StringComparison.Ordinal);

            public void Log<TState>(LogLevel nivel, EventId eventId, TState state, Exception? excepcion, Func<TState, Exception?, string> formateador)
            {
                if (!IsEnabled(nivel))
                {
                    return;
                }

                var contexto = _proveedor._httpContextAccessor.HttpContext;

                _proveedor._cola.Canal.Writer.TryWrite(new RegistroError
                {
                    Mensaje = excepcion?.Message ?? formateador(state, excepcion),
                    Detalle = $"[{_categoria}] {formateador(state, excepcion)}{Environment.NewLine}{excepcion}",
                    Ruta = contexto is null ? string.Empty : $"{contexto.Request.Method} {contexto.Request.Path}",
                    UsuarioId = contexto?.User.FindFirstValue(ClaimTypes.NameIdentifier),
                    Fecha = DateTime.UtcNow
                });
            }
        }
    }
}
