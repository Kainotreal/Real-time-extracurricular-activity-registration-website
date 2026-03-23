using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyHoatDongNgoaiKhoa.Migrations
{
    /// <inheritdoc />
    public partial class FixDangKyThamGiaCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DangKyThamGias_AspNetUsers_UserId",
                table: "DangKyThamGias");

            migrationBuilder.AddForeignKey(
                name: "FK_DangKyThamGias_AspNetUsers_UserId",
                table: "DangKyThamGias",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DangKyThamGias_AspNetUsers_UserId",
                table: "DangKyThamGias");

            migrationBuilder.AddForeignKey(
                name: "FK_DangKyThamGias_AspNetUsers_UserId",
                table: "DangKyThamGias",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
