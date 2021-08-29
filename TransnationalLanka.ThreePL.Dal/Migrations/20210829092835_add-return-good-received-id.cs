using Microsoft.EntityFrameworkCore.Migrations;

namespace TransnationalLanka.ThreePL.Dal.Migrations
{
    public partial class addreturngoodreceivedid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ReturnGoodReceivedNoteId",
                table: "GoodReceivedNotes",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GoodReceivedNotes_ReturnGoodReceivedNoteId",
                table: "GoodReceivedNotes",
                column: "ReturnGoodReceivedNoteId");

            migrationBuilder.AddForeignKey(
                name: "FK_GoodReceivedNotes_GoodReceivedNotes_ReturnGoodReceivedNoteId",
                table: "GoodReceivedNotes",
                column: "ReturnGoodReceivedNoteId",
                principalTable: "GoodReceivedNotes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GoodReceivedNotes_GoodReceivedNotes_ReturnGoodReceivedNoteId",
                table: "GoodReceivedNotes");

            migrationBuilder.DropIndex(
                name: "IX_GoodReceivedNotes_ReturnGoodReceivedNoteId",
                table: "GoodReceivedNotes");

            migrationBuilder.DropColumn(
                name: "ReturnGoodReceivedNoteId",
                table: "GoodReceivedNotes");
        }
    }
}
