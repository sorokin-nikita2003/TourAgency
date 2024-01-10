using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace agency.Data.Migrations
{
    public partial class MinusCard : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "cartNum",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "cvv",
                table: "Orders");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "cartNum",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cvv",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
