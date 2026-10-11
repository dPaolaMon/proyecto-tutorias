using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tutorias.Migrations
{
    /// <inheritdoc />
    public partial class AddUserSoftDeleteAndStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaBaja",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotivoBaja",
                table: "AspNetUsers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Activo",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FechaBaja",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MotivoBaja",
                table: "AspNetUsers");
        }
    }
}
