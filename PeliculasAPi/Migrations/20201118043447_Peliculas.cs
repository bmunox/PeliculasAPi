using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PeliculasAPi.Migrations
{
    public partial class Peliculas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Actores",
                maxLength: 120,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Peliculas",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(maxLength: 300, nullable: false),
                    EnCines = table.Column<bool>(nullable: false),
                    FechaEstreno = table.Column<DateTime>(nullable: false),
                    Poster = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Peliculas", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Peliculas");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Actores",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 120);
        }
    }
}
