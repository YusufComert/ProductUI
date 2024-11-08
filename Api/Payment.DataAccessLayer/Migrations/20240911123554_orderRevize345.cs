using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class orderRevize345 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductSales_Products_ProductId",
                table: "ProductSales");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductSales",
                table: "ProductSales");

            migrationBuilder.RenameTable(
                name: "ProductSales",
                newName: "ProductSaless");

            migrationBuilder.RenameIndex(
                name: "IX_ProductSales_ProductId",
                table: "ProductSaless",
                newName: "IX_ProductSaless_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductSaless",
                table: "ProductSaless",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSaless_Products_ProductId",
                table: "ProductSaless",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductSaless_Products_ProductId",
                table: "ProductSaless");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductSaless",
                table: "ProductSaless");

            migrationBuilder.RenameTable(
                name: "ProductSaless",
                newName: "ProductSales");

            migrationBuilder.RenameIndex(
                name: "IX_ProductSaless_ProductId",
                table: "ProductSales",
                newName: "IX_ProductSales_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductSales",
                table: "ProductSales",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSales_Products_ProductId",
                table: "ProductSales",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
