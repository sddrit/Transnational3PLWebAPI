using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TransnationalLanka.ThreePL.Dal.Migrations
{
    public partial class changethedeliveryentity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrackingNumbers",
                table: "Deliveries");

            migrationBuilder.CreateTable(
                name: "DeliveryTrackings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeliveryId = table.Column<long>(type: "bigint", nullable: false),
                    TrackingNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Updated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryTrackings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeliveryTrackings_Deliveries_DeliveryId",
                        column: x => x.DeliveryId,
                        principalTable: "Deliveries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryTrackingItem",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DeliveryTrackingId = table.Column<long>(type: "bigint", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Updated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryTrackingItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeliveryTrackingItem_DeliveryTrackings_DeliveryTrackingId",
                        column: x => x.DeliveryTrackingId,
                        principalTable: "DeliveryTrackings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeliveryTrackingItem_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryTrackingItem_DeliveryTrackingId",
                table: "DeliveryTrackingItem",
                column: "DeliveryTrackingId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryTrackingItem_ProductId",
                table: "DeliveryTrackingItem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryTrackings_DeliveryId",
                table: "DeliveryTrackings",
                column: "DeliveryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeliveryTrackingItem");

            migrationBuilder.DropTable(
                name: "DeliveryTrackings");

            migrationBuilder.AddColumn<string>(
                name: "TrackingNumbers",
                table: "Deliveries",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
