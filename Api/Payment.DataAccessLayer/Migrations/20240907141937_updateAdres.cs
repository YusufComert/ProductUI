using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class updateAdres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // AppUserId kolonunu Address tablosuna ekliyoruz
            migrationBuilder.AddColumn<int>(
                name: "AppUserId",
                table: "Addresses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // AppUserId için bir index oluşturuyoruz
            migrationBuilder.CreateIndex(
                name: "IX_Addresses_AppUserId",
                table: "Addresses",
                column: "AppUserId");

            // AppUserId ile AspNetUsers (AppUser tablosu) arasında bir foreign key oluşturuyoruz
            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_AspNetUsers_AppUserId",
                table: "Addresses",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Foreign key'i ve index'i kaldırıyoruz
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_AspNetUsers_AppUserId",
                table: "Addresses");

            migrationBuilder.DropIndex(
                name: "IX_Addresses_AppUserId",
                table: "Addresses");

            // AppUserId kolonunu kaldırıyoruz
            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Addresses");
        }
    }
}
