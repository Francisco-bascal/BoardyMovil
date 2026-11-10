using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Boardy.Backend.Migrations
{
    /// <inheritdoc />
    public partial class EstadosPorTableroYRegistroErrores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EstadoTareas_Nombre",
                table: "EstadoTareas");

            migrationBuilder.AddColumn<bool>(
                name: "EsBase",
                table: "EstadoTareas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            // Nullable al principio: los estados globales existentes todavía no tienen tablero.
            migrationBuilder.AddColumn<int>(
                name: "TableroId",
                table: "EstadoTareas",
                type: "int",
                nullable: true);

            // Cada tablero recibe su copia de los 3 estados base y de los estados globales que ya usa.
            migrationBuilder.Sql(@"
INSERT INTO EstadoTareas (Nombre, Descripcion, EsBase, TableroId)
SELECT e.Nombre, e.Descripcion,
       CASE WHEN e.Nombre IN (N'Pendiente', N'En progreso', N'Finalizada') THEN 1 ELSE 0 END,
       t.TableroId
FROM Tableros t
CROSS JOIN EstadoTareas e
WHERE e.TableroId IS NULL
  AND (e.Nombre IN (N'Pendiente', N'En progreso', N'Finalizada')
       OR EXISTS (SELECT 1 FROM ListaTareas l WHERE l.TableroId = t.TableroId AND l.EstadoTareaId = e.EstadoTareaId)
       OR EXISTS (SELECT 1 FROM Tareas ta JOIN ListaTareas l ON ta.ListaTareaId = l.ListaTareaId
                  WHERE l.TableroId = t.TableroId AND ta.EstadoTareaId = e.EstadoTareaId));

UPDATE l SET l.EstadoTareaId = n.EstadoTareaId
FROM ListaTareas l
JOIN EstadoTareas v ON l.EstadoTareaId = v.EstadoTareaId AND v.TableroId IS NULL
JOIN EstadoTareas n ON n.TableroId = l.TableroId AND n.Nombre = v.Nombre;

UPDATE ta SET ta.EstadoTareaId = n.EstadoTareaId
FROM Tareas ta
JOIN ListaTareas l ON ta.ListaTareaId = l.ListaTareaId
JOIN EstadoTareas v ON ta.EstadoTareaId = v.EstadoTareaId AND v.TableroId IS NULL
JOIN EstadoTareas n ON n.TableroId = l.TableroId AND n.Nombre = v.Nombre;

DELETE FROM EstadoTareas WHERE TableroId IS NULL;");

            migrationBuilder.AlterColumn<int>(
                name: "TableroId",
                table: "EstadoTareas",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "RegistrosError",
                columns: table => new
                {
                    RegistroErrorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Mensaje = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Detalle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ruta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsuarioId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosError", x => x.RegistroErrorId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EstadoTareas_TableroId_Nombre",
                table: "EstadoTareas",
                columns: new[] { "TableroId", "Nombre" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosError_Fecha",
                table: "RegistrosError",
                column: "Fecha");

            migrationBuilder.AddForeignKey(
                name: "FK_EstadoTareas_Tableros_TableroId",
                table: "EstadoTareas",
                column: "TableroId",
                principalTable: "Tableros",
                principalColumn: "TableroId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EstadoTareas_Tableros_TableroId",
                table: "EstadoTareas");

            migrationBuilder.DropTable(
                name: "RegistrosError");

            migrationBuilder.DropIndex(
                name: "IX_EstadoTareas_TableroId_Nombre",
                table: "EstadoTareas");

            migrationBuilder.DropColumn(
                name: "EsBase",
                table: "EstadoTareas");

            migrationBuilder.DropColumn(
                name: "TableroId",
                table: "EstadoTareas");

            migrationBuilder.CreateIndex(
                name: "IX_EstadoTareas_Nombre",
                table: "EstadoTareas",
                column: "Nombre",
                unique: true);
        }
    }
}
