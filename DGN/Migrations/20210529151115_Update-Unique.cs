using Microsoft.EntityFrameworkCore.Migrations;

namespace DGN.Migrations
{
    public partial class UpdateUnique : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Article_Title",
                table: "Article",
                column: "Title",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Article_Title",
                table: "Article");
        }
    }
}
