using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TransnationalLanka.ThreePL.Dal.Migrations
{
    public partial class changesforstock : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductStockAdjustments_GoodReceivedNotes_GoodReceivedNoteId",
                table: "ProductStockAdjustments");

            migrationBuilder.DropIndex(
                name: "IX_ProductStockAdjustments_GoodReceivedNoteId",
                table: "ProductStockAdjustments");

            migrationBuilder.DropColumn(
                name: "GoodReceivedNoteId",
                table: "ProductStockAdjustments");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpiredDate",
                table: "ProductStockAdjustments",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "ProductStockAdjustments",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Note",
                table: "ProductStockAdjustments");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpiredDate",
                table: "ProductStockAdjustments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "GoodReceivedNoteId",
                table: "ProductStockAdjustments",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductStockAdjustments_GoodReceivedNoteId",
                table: "ProductStockAdjustments",
                column: "GoodReceivedNoteId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductStockAdjustments_GoodReceivedNotes_GoodReceivedNoteId",
                table: "ProductStockAdjustments",
                column: "GoodReceivedNoteId",
                principalTable: "GoodReceivedNotes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
