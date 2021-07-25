using Microsoft.EntityFrameworkCore.Migrations;

namespace TransnationalLanka.ThreePL.Dal.Migrations
{
    public partial class adddeliverynoproperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "WareHouseId",
                table: "Deliveries",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryNo",
                table: "Deliveries",
                type: "varchar(max)",
                unicode: false,
                nullable: true,
                computedColumnSql: "('DL'+right(replicate('0',(8))+CONVERT([varchar],[Id]),(8)))");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_WareHouseId",
                table: "Deliveries",
                column: "WareHouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Deliveries_WareHouses_WareHouseId",
                table: "Deliveries",
                column: "WareHouseId",
                principalTable: "WareHouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deliveries_WareHouses_WareHouseId",
                table: "Deliveries");

            migrationBuilder.DropIndex(
                name: "IX_Deliveries_WareHouseId",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "DeliveryNo",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "WareHouseId",
                table: "Deliveries");
        }
    }
}
