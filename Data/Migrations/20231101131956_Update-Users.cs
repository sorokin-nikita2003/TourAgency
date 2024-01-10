using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace agency.Data.Migrations
{
    public partial class UpdateUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_baskets_BasketTour_BasketTourId",
                table: "baskets");

            migrationBuilder.DropForeignKey(
                name: "FK_baskets_Tour_TourId",
                table: "baskets");

            migrationBuilder.DropTable(
                name: "BasketTour");

            migrationBuilder.DropIndex(
                name: "IX_baskets_BasketTourId",
                table: "baskets");

            migrationBuilder.DropColumn(
                name: "BasketTourId",
                table: "baskets");

            migrationBuilder.AlterColumn<int>(
                name: "TourId",
                table: "baskets",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "baskets",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_baskets_UserId",
                table: "baskets",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_baskets_AspNetUsers_UserId",
                table: "baskets",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_baskets_Tour_TourId",
                table: "baskets",
                column: "TourId",
                principalTable: "Tour",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_baskets_AspNetUsers_UserId",
                table: "baskets");

            migrationBuilder.DropForeignKey(
                name: "FK_baskets_Tour_TourId",
                table: "baskets");

            migrationBuilder.DropIndex(
                name: "IX_baskets_UserId",
                table: "baskets");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "baskets");

            migrationBuilder.AlterColumn<int>(
                name: "TourId",
                table: "baskets",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BasketTourId",
                table: "baskets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "BasketTour",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasketTour", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_baskets_BasketTourId",
                table: "baskets",
                column: "BasketTourId");

            migrationBuilder.AddForeignKey(
                name: "FK_baskets_BasketTour_BasketTourId",
                table: "baskets",
                column: "BasketTourId",
                principalTable: "BasketTour",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_baskets_Tour_TourId",
                table: "baskets",
                column: "TourId",
                principalTable: "Tour",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
