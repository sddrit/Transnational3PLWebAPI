using Microsoft.EntityFrameworkCore.Migrations;

namespace TransnationalLanka.ThreePL.Dal.Migrations
{
    public partial class changepropertynamesforcost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Cost",
                table: "PurchaseOrderItems",
                newName: "UnitCost");

            migrationBuilder.RenameColumn(
                name: "UnitPrice",
                table: "ProductStockAdjustments",
                newName: "UnitCost");

            migrationBuilder.RenameColumn(
                name: "Cost",
                table: "GoodReceivedNoteItems",
                newName: "UnitCost");

            migrationBuilder.AddColumn<long>(
                name: "WareHouseId",
                table: "ProductStockAdjustments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_ProductStockAdjustments_WareHouseId",
                table: "ProductStockAdjustments",
                column: "WareHouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductStockAdjustments_WareHouses_WareHouseId",
                table: "ProductStockAdjustments",
                column: "WareHouseId",
                principalTable: "WareHouses",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductStockAdjustments_WareHouses_WareHouseId",
                table: "ProductStockAdjustments");

            migrationBuilder.DropIndex(
                name: "IX_ProductStockAdjustments_WareHouseId",
                table: "ProductStockAdjustments");

            migrationBuilder.DropColumn(
                name: "WareHouseId",
                table: "ProductStockAdjustments");

            migrationBuilder.RenameColumn(
                name: "UnitCost",
                table: "PurchaseOrderItems",
                newName: "Cost");

            migrationBuilder.RenameColumn(
                name: "UnitCost",
                table: "ProductStockAdjustments",
                newName: "UnitPrice");

            migrationBuilder.RenameColumn(
                name: "UnitCost",
                table: "GoodReceivedNoteItems",
                newName: "Cost");
        }
    }
}
