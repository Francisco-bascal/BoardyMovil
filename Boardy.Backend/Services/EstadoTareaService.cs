using Boardy.Backend.Comun;
using Boardy.Backend.Data;
using Boardy.Backend.DTOs;
using Boardy.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Boardy.Backend.Services
{
    /// <summary>
    /// Estados de tarea propios de cada tablero. Los 3 estados base nacen con el tablero y son fijos.
    /// </summary>
    public class EstadoTareaService : IEstadoTareaService
    {
        private static readonly Dictionary<string, string> DescripcionesBase = new()
        {
            [EstadosTareaSistema.Pendiente] = "La tarea todavía no comenzó.",
            [EstadosTareaSistema.EnProgreso] = "La tarea está siendo trabajada.",
            [EstadosTareaSistema.Finalizada] = "La tarea fue completada."
        };

        private readonly AppDbContext _context;
        private readonly IAuditoriaService _auditoria;

        public EstadoTareaService(AppDbContext context, IAuditoriaService auditoria)
        {
            _context = context;
            _auditoria = auditoria;
        }

        public async Task<IReadOnlyList<EstadoTareaDto>> ListarAsync(int tableroId, CancellationToken cancellationToken = default)
        {
            return await _context.EstadoTareas
                .AsNoTracking()
                .Where(e => e.TableroId == tableroId)
                .OrderByDescending(e => e.EsBase)
                .ThenBy(e => e.EstadoTareaId)
                .Select(e => new EstadoTareaDto
                {
                    EstadoTareaId = e.EstadoTareaId,
                    Nombre = e.Nombre,
                    Descripcion = e.Descripcion,
                    CantidadTareas = e.Tareas.Count,
                    EsBase = e.EsBase
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<int?> ObtenerTableroIdAsync(int estadoTareaId, CancellationToken cancellationToken = default)
        {
            return await _context.EstadoTareas
                .Where(e => e.EstadoTareaId == estadoTareaId)
                .Select(e => (int?)e.TableroId)
                .FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>Los 3 estados base (Pendiente, En progreso, Finalizada) para un tablero nuevo, sin guardar.</summary>
        public static List<EstadoTarea> CrearEstadosBase() => EstadosTareaSistema.Todos
            .Select(nombre => new EstadoTarea { Nombre = nombre, Descripcion = DescripcionesBase[nombre], EsBase = true })
            .ToList();

        public async Task<ResultadoOperacion<EstadoTareaDto>> CrearAsync(int tableroId, CrearEstadoTareaDto dto, CancellationToken cancellationToken = default)
        {
            var nombre = dto.Nombre.Trim();

            // Regla de nombre único: no puede repetirse dentro del tablero.
            if (await NombreEnUsoAsync(tableroId, nombre, null, cancellationToken))
            {
                return ResultadoOperacion<EstadoTareaDto>.Fallo(nameof(dto.Nombre), "Ya existe un estado con ese nombre en el tablero.");
            }

            var estado = new EstadoTarea { TableroId = tableroId, Nombre = nombre, Descripcion = dto.Descripcion.Trim() };
            _context.EstadoTareas.Add(estado);
            _auditoria.Registrar(tableroId, $"Creó el estado \"{nombre}\".");
            await _context.SaveChangesAsync(cancellationToken);

            return ResultadoOperacion<EstadoTareaDto>.Exitoso(Mapear(estado, 0), "Estado creado correctamente.");
        }

        public async Task<ResultadoOperacion<EstadoTareaDto>> ActualizarAsync(int estadoTareaId, ActualizarEstadoTareaDto dto, CancellationToken cancellationToken = default)
        {
            var estado = await _context.EstadoTareas.FirstOrDefaultAsync(e => e.EstadoTareaId == estadoTareaId, cancellationToken);

            if (estado is null)
            {
                return ResultadoOperacion<EstadoTareaDto>.NoExiste("El estado no existe.");
            }

            // Regla de estados base: Kanban y el semáforo de vencimiento dependen de sus nombres.
            if (estado.EsBase)
            {
                return ResultadoOperacion<EstadoTareaDto>.Fallo("Los estados base del tablero no se pueden modificar.");
            }

            var nombre = dto.Nombre.Trim();
            if (await NombreEnUsoAsync(estado.TableroId, nombre, estadoTareaId, cancellationToken))
            {
                return ResultadoOperacion<EstadoTareaDto>.Fallo(nameof(dto.Nombre), "Ya existe un estado con ese nombre en el tablero.");
            }

            estado.Nombre = nombre;
            estado.Descripcion = dto.Descripcion.Trim();
            _auditoria.Registrar(estado.TableroId, $"Editó el estado \"{nombre}\".");
            await _context.SaveChangesAsync(cancellationToken);

            var cantidadTareas = await _context.Tareas.CountAsync(t => t.EstadoTareaId == estadoTareaId, cancellationToken);
            return ResultadoOperacion<EstadoTareaDto>.Exitoso(Mapear(estado, cantidadTareas), "Estado actualizado correctamente.");
        }

        public async Task<ResultadoOperacion> EliminarAsync(int estadoTareaId, CancellationToken cancellationToken = default)
        {
            var estado = await _context.EstadoTareas.FirstOrDefaultAsync(e => e.EstadoTareaId == estadoTareaId, cancellationToken);

            if (estado is null)
            {
                return ResultadoOperacion.NoExiste("El estado no existe.");
            }

            if (estado.EsBase)
            {
                return ResultadoOperacion.Fallo("Los estados base del tablero no se pueden eliminar.");
            }

            // Regla de estado en uso: no se elimina si una lista o una tarea lo usa.
            if (await _context.ListaTareas.AnyAsync(l => l.EstadoTareaId == estadoTareaId, cancellationToken)
                || await _context.Tareas.AnyAsync(t => t.EstadoTareaId == estadoTareaId, cancellationToken))
            {
                return ResultadoOperacion.Fallo("No se puede eliminar: el estado está asignado a una lista o a una tarea.");
            }

            _context.EstadoTareas.Remove(estado);
            _auditoria.Registrar(estado.TableroId, $"Eliminó el estado \"{estado.Nombre}\".");
            await _context.SaveChangesAsync(cancellationToken);

            return ResultadoOperacion.Exitoso("Estado eliminado correctamente.");
        }

        private Task<bool> NombreEnUsoAsync(int tableroId, string nombre, int? estadoTareaIdExcluido, CancellationToken cancellationToken) =>
            _context.EstadoTareas.AnyAsync(e => e.TableroId == tableroId && e.Nombre == nombre && e.EstadoTareaId != estadoTareaIdExcluido, cancellationToken);

        private static EstadoTareaDto Mapear(EstadoTarea estado, int cantidadTareas) => new()
        {
            EstadoTareaId = estado.EstadoTareaId,
            Nombre = estado.Nombre,
            Descripcion = estado.Descripcion,
            CantidadTareas = cantidadTareas,
            EsBase = estado.EsBase
        };
    }
}
