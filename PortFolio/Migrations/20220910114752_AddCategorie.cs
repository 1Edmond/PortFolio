using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortFolio.Migrations
{
    public partial class AddCategorie : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateFin",
                table: "Projets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateFin",
                table: "Projets");
        }
    }
}
