using Microsoft.EntityFrameworkCore.Migrations;

namespace TransnationalLanka.ThreePL.Dal.Migrations
{
    public partial class addproductforstockadjustment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductStockAdjustments_Products_ProductId",
                table: "ProductStockAdjustments");

            migrationBuilder.AlterColumn<long>(
                name: "ProductId",
                table: "ProductStockAdjustments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductStockAdjustments_Products_ProductId",
                table: "ProductStockAdjustments",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductStockAdjustments_Products_ProductId",
                table: "ProductStockAdjustments");

            migrationBuilder.AlterColumn<long>(
                name: "ProductId",
                table: "ProductStockAdjustments",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductStockAdjustments_Products_ProductId",
                table: "ProductStockAdjustments",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
