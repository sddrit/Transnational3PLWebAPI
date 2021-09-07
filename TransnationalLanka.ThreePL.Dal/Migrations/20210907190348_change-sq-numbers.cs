using Microsoft.EntityFrameworkCore.Migrations;

namespace TransnationalLanka.ThreePL.Dal.Migrations
{
    public partial class changesqnumbers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "StockTransferNumber",
                table: "StockTransfers",
                type: "varchar(max)",
                unicode: false,
                nullable: true,
                computedColumnSql: "([dbo].[FN_GENERATE_ST_NUMBER]([Id]))",
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false,
                oldNullable: true,
                oldComputedColumnSql: "('ST'+right(replicate('0',(8))+CONVERT([varchar],[Id]),(8)))");

            migrationBuilder.AlterColumn<string>(
                name: "PoNumber",
                table: "PurchaseOrders",
                type: "varchar(max)",
                unicode: false,
                nullable: true,
                computedColumnSql: "([dbo].[FN_GENERATE_PO_NUMBER]([Id]))",
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false,
                oldNullable: true,
                oldComputedColumnSql: "('PO'+right(replicate('0',(8))+CONVERT([varchar],[Id]),(8)))");

            migrationBuilder.AlterColumn<string>(
                name: "InvoiceNo",
                table: "Invoices",
                type: "varchar(max)",
                unicode: false,
                nullable: true,
                computedColumnSql: "([dbo].[FN_GENERATE_INVOICE_NUMBER]([Id]))",
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false,
                oldNullable: true,
                oldComputedColumnSql: "('IN'+right(replicate('0',(8))+CONVERT([varchar],[Id]),(8)))");

            migrationBuilder.AlterColumn<string>(
                name: "GrnNo",
                table: "GoodReceivedNotes",
                type: "varchar(max)",
                unicode: false,
                nullable: true,
                computedColumnSql: "([dbo].[FN_GENERATE_GRN_NUMBER]([Id]))",
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false,
                oldNullable: true,
                oldComputedColumnSql: "('GRN'+right(replicate('0',(8))+CONVERT([varchar],[Id]),(8)))");

            migrationBuilder.AlterColumn<string>(
                name: "DeliveryNo",
                table: "Deliveries",
                type: "varchar(max)",
                unicode: false,
                nullable: true,
                computedColumnSql: "([dbo].[FN_GENERATE_DELIVERY_NUMBER]([Id]))",
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false,
                oldNullable: true,
                oldComputedColumnSql: "('DL'+right(replicate('0',(8))+CONVERT([varchar],[Id]),(8)))");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "StockTransferNumber",
                table: "StockTransfers",
                type: "varchar(max)",
                unicode: false,
                nullable: true,
                computedColumnSql: "('ST'+right(replicate('0',(8))+CONVERT([varchar],[Id]),(8)))",
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false,
                oldNullable: true,
                oldComputedColumnSql: "([dbo].[FN_GENERATE_ST_NUMBER]([Id]))");

            migrationBuilder.AlterColumn<string>(
                name: "PoNumber",
                table: "PurchaseOrders",
                type: "varchar(max)",
                unicode: false,
                nullable: true,
                computedColumnSql: "('PO'+right(replicate('0',(8))+CONVERT([varchar],[Id]),(8)))",
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false,
                oldNullable: true,
                oldComputedColumnSql: "([dbo].[FN_GENERATE_PO_NUMBER]([Id]))");

            migrationBuilder.AlterColumn<string>(
                name: "InvoiceNo",
                table: "Invoices",
                type: "varchar(max)",
                unicode: false,
                nullable: true,
                computedColumnSql: "('IN'+right(replicate('0',(8))+CONVERT([varchar],[Id]),(8)))",
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false,
                oldNullable: true,
                oldComputedColumnSql: "([dbo].[FN_GENERATE_INVOICE_NUMBER]([Id]))");

            migrationBuilder.AlterColumn<string>(
                name: "GrnNo",
                table: "GoodReceivedNotes",
                type: "varchar(max)",
                unicode: false,
                nullable: true,
                computedColumnSql: "('GRN'+right(replicate('0',(8))+CONVERT([varchar],[Id]),(8)))",
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false,
                oldNullable: true,
                oldComputedColumnSql: "([dbo].[FN_GENERATE_GRN_NUMBER]([Id]))");

            migrationBuilder.AlterColumn<string>(
                name: "DeliveryNo",
                table: "Deliveries",
                type: "varchar(max)",
                unicode: false,
                nullable: true,
                computedColumnSql: "('DL'+right(replicate('0',(8))+CONVERT([varchar],[Id]),(8)))",
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false,
                oldNullable: true,
                oldComputedColumnSql: "([dbo].[FN_GENERATE_DELIVERY_NUMBER]([Id]))");
        }
    }
}
