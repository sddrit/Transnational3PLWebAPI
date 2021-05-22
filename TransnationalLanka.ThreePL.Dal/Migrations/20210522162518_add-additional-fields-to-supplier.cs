using Microsoft.EntityFrameworkCore.Migrations;

namespace TransnationalLanka.ThreePL.Dal.Migrations
{
    public partial class addadditionalfieldstosupplier : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Suppliers",
                newName: "VatNumber");

            migrationBuilder.AddColumn<long>(
                name: "AddressId",
                table: "Suppliers",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BusinessRegistrationId",
                table: "Suppliers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Contact_ContactPerson",
                table: "Suppliers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Contact_Email",
                table: "Suppliers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Contact_Mobile",
                table: "Suppliers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Contact_Phone",
                table: "Suppliers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoicePolicy",
                table: "Suppliers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReturnPolicy",
                table: "Suppliers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SupplierCharges_AdditionalChargePerUnitPrice",
                table: "Suppliers",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SupplierCharges_AllocatedUnits",
                table: "Suppliers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SupplierCharges_AllocatedUnitsFixedPrice",
                table: "Suppliers",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SupplierCharges_HandlingCharge",
                table: "Suppliers",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SupplierName",
                table: "Suppliers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_AddressId",
                table: "Suppliers",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Suppliers_Addresses_AddressId",
                table: "Suppliers",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Suppliers_Addresses_AddressId",
                table: "Suppliers");

            migrationBuilder.DropIndex(
                name: "IX_Suppliers_AddressId",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "BusinessRegistrationId",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "Contact_ContactPerson",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "Contact_Email",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "Contact_Mobile",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "Contact_Phone",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "InvoicePolicy",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "ReturnPolicy",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "SupplierCharges_AdditionalChargePerUnitPrice",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "SupplierCharges_AllocatedUnits",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "SupplierCharges_AllocatedUnitsFixedPrice",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "SupplierCharges_HandlingCharge",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "SupplierName",
                table: "Suppliers");

            migrationBuilder.RenameColumn(
                name: "VatNumber",
                table: "Suppliers",
                newName: "Name");
        }
    }
}
