using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tutorias.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTutorMateriaYTutorHorario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SalonAsignado",
                table: "TutoresPerfiles");

            migrationBuilder.CreateTable(
                name: "TutoresHorarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TutorPerfilId = table.Column<int>(type: "int", nullable: false),
                    DiaSemana = table.Column<int>(type: "int", nullable: false),
                    HoraInicio = table.Column<TimeSpan>(type: "time", nullable: false),
                    HoraFin = table.Column<TimeSpan>(type: "time", nullable: false),
                    Lugar = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MateriaId = table.Column<int>(type: "int", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TutoresHorarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TutoresHorarios_Materias_MateriaId",
                        column: x => x.MateriaId,
                        principalTable: "Materias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TutoresHorarios_TutoresPerfiles_TutorPerfilId",
                        column: x => x.TutorPerfilId,
                        principalTable: "TutoresPerfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TutoresMaterias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TutorPerfilId = table.Column<int>(type: "int", nullable: false),
                    MateriaId = table.Column<int>(type: "int", nullable: false),
                    FechaAsignacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TutoresMaterias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TutoresMaterias_Materias_MateriaId",
                        column: x => x.MateriaId,
                        principalTable: "Materias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TutoresMaterias_TutoresPerfiles_TutorPerfilId",
                        column: x => x.TutorPerfilId,
                        principalTable: "TutoresPerfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TutoresHorarios_MateriaId",
                table: "TutoresHorarios",
                column: "MateriaId");

            migrationBuilder.CreateIndex(
                name: "IX_TutoresHorarios_TutorPerfilId",
                table: "TutoresHorarios",
                column: "TutorPerfilId");

            migrationBuilder.CreateIndex(
                name: "IX_TutoresMaterias_MateriaId",
                table: "TutoresMaterias",
                column: "MateriaId");

            migrationBuilder.CreateIndex(
                name: "IX_TutoresMaterias_TutorPerfilId_MateriaId",
                table: "TutoresMaterias",
                columns: new[] { "TutorPerfilId", "MateriaId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TutoresHorarios");

            migrationBuilder.DropTable(
                name: "TutoresMaterias");

            migrationBuilder.AddColumn<string>(
                name: "SalonAsignado",
                table: "TutoresPerfiles",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
