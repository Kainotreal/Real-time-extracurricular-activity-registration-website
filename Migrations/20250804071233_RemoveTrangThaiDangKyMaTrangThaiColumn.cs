using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyHoatDongNgoaiKhoa.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTrangThaiDangKyMaTrangThaiColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Xóa foreign key trước
            migrationBuilder.DropForeignKey(
                name: "FK_DangKyThamGias_TrangThaiDangKys_TrangThaiDangKyMaTrangThai",
                table: "DangKyThamGias");

            // Xóa index
            migrationBuilder.DropIndex(
                name: "IX_DangKyThamGias_TrangThaiDangKyMaTrangThai",
                table: "DangKyThamGias");

            // Xóa cột TrangThaiDangKyMaTrangThai vì đã có cột MaTrangThaiDangKy
            migrationBuilder.DropColumn(
                name: "TrangThaiDangKyMaTrangThai",
                table: "DangKyThamGias");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Thêm lại cột TrangThaiDangKyMaTrangThai nếu cần rollback
            migrationBuilder.AddColumn<int>(
                name: "TrangThaiDangKyMaTrangThai",
                table: "DangKyThamGias",
                type: "int",
                nullable: true);

            // Tạo lại index
            migrationBuilder.CreateIndex(
                name: "IX_DangKyThamGias_TrangThaiDangKyMaTrangThai",
                table: "DangKyThamGias",
                column: "TrangThaiDangKyMaTrangThai");

            // Tạo lại foreign key
            migrationBuilder.AddForeignKey(
                name: "FK_DangKyThamGias_TrangThaiDangKys_TrangThaiDangKyMaTrangThai",
                table: "DangKyThamGias",
                column: "TrangThaiDangKyMaTrangThai",
                principalTable: "TrangThaiDangKys",
                principalColumn: "MaTrangThaiDangKy",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
