using Microsoft.EntityFrameworkCore.Migrations;

namespace TransnationalLanka.ThreePL.Dal.Migrations
{
    public partial class invoicechanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "InvoiceItems",
                newName: "Rate");

            migrationBuilder.AddColumn<int>(
                name: "TaxType",
                table: "Invoices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Quantity",
                table: "InvoiceItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaxType",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "InvoiceItems");

            migrationBuilder.RenameColumn(
                name: "Rate",
                table: "InvoiceItems",
                newName: "Amount");
        }
    }
}
