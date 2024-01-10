using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace agency.Data.Migrations
{
    public partial class AddBasket_Personscount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Basket_BasketTour_BasketTourId",
                table: "Basket");

            migrationBuilder.DropForeignKey(
                name: "FK_Basket_Tour_TourId",
                table: "Basket");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Basket",
                table: "Basket");

            migrationBuilder.RenameTable(
                name: "Basket",
                newName: "baskets");

            migrationBuilder.RenameIndex(
                name: "IX_Basket_TourId",
                table: "baskets",
                newName: "IX_baskets_TourId");

            migrationBuilder.RenameIndex(
                name: "IX_Basket_BasketTourId",
                table: "baskets",
                newName: "IX_baskets_BasketTourId");

            migrationBuilder.AddColumn<int>(
                name: "PersonsCount",
                table: "baskets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_baskets",
                table: "baskets",
                column: "Id");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_baskets_BasketTour_BasketTourId",
                table: "baskets");

            migrationBuilder.DropForeignKey(
                name: "FK_baskets_Tour_TourId",
                table: "baskets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_baskets",
                table: "baskets");

            migrationBuilder.DropColumn(
                name: "PersonsCount",
                table: "baskets");

            migrationBuilder.RenameTable(
                name: "baskets",
                newName: "Basket");

            migrationBuilder.RenameIndex(
                name: "IX_baskets_TourId",
                table: "Basket",
                newName: "IX_Basket_TourId");

            migrationBuilder.RenameIndex(
                name: "IX_baskets_BasketTourId",
                table: "Basket",
                newName: "IX_Basket_BasketTourId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Basket",
                table: "Basket",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Basket_BasketTour_BasketTourId",
                table: "Basket",
                column: "BasketTourId",
                principalTable: "BasketTour",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Basket_Tour_TourId",
                table: "Basket",
                column: "TourId",
                principalTable: "Tour",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
