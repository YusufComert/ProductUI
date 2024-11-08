using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class orderRevize3456 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "ProductSaless");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "ProductSaless",
                type: "int",
                nullable: true);
        }
    }
}
