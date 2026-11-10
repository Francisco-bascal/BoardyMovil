using Boardy.Backend.Comun;
using Boardy.Backend.Data;

namespace Boardy.Backend.Services
{
    /// <summary>Vacía la cola de errores del logger y los guarda en la base de datos.</summary>
    public class GuardadoRegistroErroresService : BackgroundService
    {
        private readonly ColaRegistroErrores _cola;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<GuardadoRegistroErroresService> _logger;

        public GuardadoRegistroErroresService(ColaRegistroErrores cola, IServiceScopeFactory scopeFactory, ILogger<GuardadoRegistroErroresService> logger)
        {
            _cola = cola;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var registro in _cola.Canal.Reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    // Un scope por registro: el DbContext de una petición fallida puede estar en un estado inválido.
                    using var scope = _scopeFactory.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    context.RegistrosError.Add(registro);
                    await context.SaveChangesAsync(stoppingToken);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    // Warning y no Error: un Error volvería a entrar en la cola que no se pudo guardar.
                    _logger.LogWarning(ex, "No se pudo guardar un registro de error en la base de datos.");
                }
            }
        }
    }
}
