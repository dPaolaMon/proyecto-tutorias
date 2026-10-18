using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tutorias.Migrations
{
    /// <inheritdoc />
    public partial class AgregarConfirmacionAsistencia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Asistencias_SesionId",
                table: "Asistencias");

            migrationBuilder.AddColumn<bool>(
                name: "Confirmada",
                table: "Asistencias",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaHoraConfirmacion",
                table: "Asistencias",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Asistencias_SesionId_AlumnoId",
                table: "Asistencias",
                columns: new[] { "SesionId", "AlumnoId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Asistencias_SesionId_AlumnoId",
                table: "Asistencias");

            migrationBuilder.DropColumn(
                name: "Confirmada",
                table: "Asistencias");

            migrationBuilder.DropColumn(
                name: "FechaHoraConfirmacion",
                table: "Asistencias");

            migrationBuilder.CreateIndex(
                name: "IX_Asistencias_SesionId",
                table: "Asistencias",
                column: "SesionId");
        }
    }
}
