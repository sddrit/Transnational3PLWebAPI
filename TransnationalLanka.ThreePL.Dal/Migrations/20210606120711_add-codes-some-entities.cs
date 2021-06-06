using Microsoft.EntityFrameworkCore.Migrations;

namespace TransnationalLanka.ThreePL.Dal.Migrations
{
    public partial class addcodessomeentities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SupplierCode",
                table: "Suppliers");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "WareHouses",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Suppliers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Products",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PoNumber",
                table: "PurchaseOrders",
                type: "varchar(max)",
                unicode: false,
                nullable: true,
                computedColumnSql: "('PO'+right(replicate('0',(8))+CONVERT([varchar],[Id]),(8)))");

            migrationBuilder.AddColumn<string>(
                name: "GrnNo",
                table: "GoodReceivedNotes",
                type: "varchar(max)",
                unicode: false,
                nullable: true,
                computedColumnSql: "('GRN'+right(replicate('0',(8))+CONVERT([varchar],[Id]),(8)))");

            migrationBuilder.CreateIndex(
                name: "IX_WareHouses_Code",
                table: "WareHouses",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_Code",
                table: "Suppliers",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Code",
                table: "Products",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WareHouses_Code",
                table: "WareHouses");

            migrationBuilder.DropIndex(
                name: "IX_Suppliers_Code",
                table: "Suppliers");

            migrationBuilder.DropIndex(
                name: "IX_Products_Code",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "WareHouses");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "PoNumber",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "GrnNo",
                table: "GoodReceivedNotes");

            migrationBuilder.AddColumn<string>(
                name: "SupplierCode",
                table: "Suppliers",
                type: "nvarchar(max)",
                nullable: true,
                computedColumnSql: "('S-'+ right(replicate('0',(8))+CONVERT([varchar],[Id]),(8)))",
                stored: true);
        }
    }
}
