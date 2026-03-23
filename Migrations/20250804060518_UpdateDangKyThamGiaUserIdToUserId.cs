using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyHoatDongNgoaiKhoa.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDangKyThamGiaUserIdToUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DangKyThamGias_AspNetUsers_Id",
                table: "DangKyThamGias");

            migrationBuilder.DropIndex(
                name: "IX_DangKyThamGias_Id",
                table: "DangKyThamGias");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "DangKyThamGias");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "DangKyThamGias",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_DangKyThamGias_UserId",
                table: "DangKyThamGias",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_DangKyThamGias_AspNetUsers_UserId",
                table: "DangKyThamGias",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DangKyThamGias_AspNetUsers_UserId",
                table: "DangKyThamGias");

            migrationBuilder.DropIndex(
                name: "IX_DangKyThamGias_UserId",
                table: "DangKyThamGias");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "DangKyThamGias");

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "DangKyThamGias",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DangKyThamGias_Id",
                table: "DangKyThamGias",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DangKyThamGias_AspNetUsers_Id",
                table: "DangKyThamGias",
                column: "Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
