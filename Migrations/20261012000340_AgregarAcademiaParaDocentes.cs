using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tutorias.Migrations
{
    /// <inheritdoc />
    public partial class AgregarAcademiaParaDocentes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Academia",
                table: "AspNetUsers",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Academia",
                table: "AspNetUsers");
        }
    }
}
