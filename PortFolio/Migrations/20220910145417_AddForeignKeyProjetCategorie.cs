using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortFolio.Migrations
{
    public partial class AddForeignKeyProjetCategorie : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategorieId",
                table: "Projets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Projets_CategorieId",
                table: "Projets",
                column: "CategorieId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projets_Categories_CategorieId",
                table: "Projets",
                column: "CategorieId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projets_Categories_CategorieId",
                table: "Projets");

            migrationBuilder.DropIndex(
                name: "IX_Projets_CategorieId",
                table: "Projets");

            migrationBuilder.DropColumn(
                name: "CategorieId",
                table: "Projets");
        }
    }
}
