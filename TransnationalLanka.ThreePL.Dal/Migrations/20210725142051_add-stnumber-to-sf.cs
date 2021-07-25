using Microsoft.EntityFrameworkCore.Migrations;

namespace TransnationalLanka.ThreePL.Dal.Migrations
{
    public partial class addstnumbertosf : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StockTransferNumber",
                table: "StockTransfers",
                type: "varchar(max)",
                unicode: false,
                nullable: true,
                computedColumnSql: "('ST'+right(replicate('0',(8))+CONVERT([varchar],[Id]),(8)))");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StockTransferNumber",
                table: "StockTransfers");
        }
    }
}
