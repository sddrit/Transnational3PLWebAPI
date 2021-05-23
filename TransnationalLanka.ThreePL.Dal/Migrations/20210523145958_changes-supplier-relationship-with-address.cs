using Microsoft.EntityFrameworkCore.Migrations;

namespace TransnationalLanka.ThreePL.Dal.Migrations
{
    public partial class changessupplierrelationshipwithaddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Cities_CityId",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Suppliers_SupplierId",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Suppliers_Addresses_AddressId",
                table: "Suppliers");

            migrationBuilder.RenameColumn(
                name: "AddressId",
                table: "Suppliers",
                newName: "Address_CityId");

            migrationBuilder.RenameIndex(
                name: "IX_Suppliers_AddressId",
                table: "Suppliers",
                newName: "IX_Suppliers_Address_CityId");

            migrationBuilder.AddColumn<string>(
                name: "Address_AddressLine1",
                table: "Suppliers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_AddressLine2",
                table: "Suppliers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_FirstName",
                table: "Suppliers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_LastName",
                table: "Suppliers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_PostalCode",
                table: "Suppliers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Cities_CityId",
                table: "Addresses",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Suppliers_SupplierId",
                table: "Addresses",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Suppliers_Cities_Address_CityId",
                table: "Suppliers",
                column: "Address_CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Cities_CityId",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Suppliers_SupplierId",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Suppliers_Cities_Address_CityId",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "Address_AddressLine1",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "Address_AddressLine2",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "Address_FirstName",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "Address_LastName",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "Address_PostalCode",
                table: "Suppliers");

            migrationBuilder.RenameColumn(
                name: "Address_CityId",
                table: "Suppliers",
                newName: "AddressId");

            migrationBuilder.RenameIndex(
                name: "IX_Suppliers_Address_CityId",
                table: "Suppliers",
                newName: "IX_Suppliers_AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Cities_CityId",
                table: "Addresses",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Suppliers_SupplierId",
                table: "Addresses",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Suppliers_Addresses_AddressId",
                table: "Suppliers",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
