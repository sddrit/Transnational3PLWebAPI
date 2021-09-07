using Microsoft.EntityFrameworkCore.Migrations;

namespace TransnationalLanka.ThreePL.Dal.Migrations
{
    public partial class addwarehousecapacity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Height",
                table: "WareHouses",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Length",
                table: "WareHouses",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Width",
                table: "WareHouses",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Height",
                table: "WareHouses");

            migrationBuilder.DropColumn(
                name: "Length",
                table: "WareHouses");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "WareHouses");
        }
    }
}
