using Microsoft.EntityFrameworkCore.Migrations;

namespace TransnationalLanka.ThreePL.Dal.Migrations
{
    public partial class addpaidtoinvoice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Paid",
                table: "Invoices",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Paid",
                table: "Invoices");
        }
    }
}
