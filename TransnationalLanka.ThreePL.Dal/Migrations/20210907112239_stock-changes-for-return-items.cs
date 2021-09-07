using Microsoft.EntityFrameworkCore.Migrations;

namespace TransnationalLanka.ThreePL.Dal.Migrations
{
    public partial class stockchangesforreturnitems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReturnQuantity",
                table: "ProductStocks",
                newName: "SalesReturnQuantity");

            migrationBuilder.AddColumn<decimal>(
                name: "DamageStockQuantity",
                table: "ProductStocks",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DispatchReturnQuantity",
                table: "ProductStocks",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DamageStockQuantity",
                table: "ProductStocks");

            migrationBuilder.DropColumn(
                name: "DispatchReturnQuantity",
                table: "ProductStocks");

            migrationBuilder.RenameColumn(
                name: "SalesReturnQuantity",
                table: "ProductStocks",
                newName: "ReturnQuantity");
        }
    }
}
