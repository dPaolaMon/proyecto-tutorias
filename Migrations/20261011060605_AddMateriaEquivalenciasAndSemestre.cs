using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tutorias.Migrations
{
    /// <inheritdoc />
    public partial class AddMateriaEquivalenciasAndSemestre : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CodigoEquivalencia",
                table: "Materias",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Semestre",
                table: "Materias",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Activo",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.CreateTable(
                name: "MateriasEquivalencias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MateriaOrigenId = table.Column<int>(type: "int", nullable: false),
                    MateriaEquivalenteId = table.Column<int>(type: "int", nullable: false),
                    EsBidireccional = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MateriasEquivalencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MateriasEquivalencias_Materias_MateriaEquivalenteId",
                        column: x => x.MateriaEquivalenteId,
                        principalTable: "Materias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MateriasEquivalencias_Materias_MateriaOrigenId",
                        column: x => x.MateriaOrigenId,
                        principalTable: "Materias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Materias_CodigoEquivalencia",
                table: "Materias",
                column: "CodigoEquivalencia");

            migrationBuilder.CreateIndex(
                name: "IX_MateriasEquivalencias_MateriaEquivalenteId",
                table: "MateriasEquivalencias",
                column: "MateriaEquivalenteId");

            migrationBuilder.CreateIndex(
                name: "IX_MateriasEquivalencias_MateriaOrigenId",
                table: "MateriasEquivalencias",
                column: "MateriaOrigenId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MateriasEquivalencias");

            migrationBuilder.DropIndex(
                name: "IX_Materias_CodigoEquivalencia",
                table: "Materias");

            migrationBuilder.DropColumn(
                name: "CodigoEquivalencia",
                table: "Materias");

            migrationBuilder.DropColumn(
                name: "Semestre",
                table: "Materias");

            migrationBuilder.AlterColumn<bool>(
                name: "Activo",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);
        }
    }
}
