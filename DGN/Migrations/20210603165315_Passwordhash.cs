using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DGN.Migrations
{
    public partial class Passwordhash : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Hash",
                table: "Password",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Salt",
                table: "Password",
                type: "varbinary(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hash",
                table: "Password");

            migrationBuilder.DropColumn(
                name: "Salt",
                table: "Password");
        }
    }
}
