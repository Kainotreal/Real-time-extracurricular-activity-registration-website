using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyHoatDongNgoaiKhoa.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTrangThaiDangKyColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DangKyThamGias_TrangThaiDangKys_MaTrangThai",
                table: "DangKyThamGias");

            migrationBuilder.RenameColumn(
                name: "TenTrangThai",
                table: "TrangThaiDangKys",
                newName: "TenTrangThaiDangKy");

            migrationBuilder.RenameColumn(
                name: "MaTrangThai",
                table: "TrangThaiDangKys",
                newName: "MaTrangThaiDangKy");

            migrationBuilder.RenameColumn(
                name: "MaTrangThai",
                table: "DangKyThamGias",
                newName: "MaTrangThaiDangKy");

            migrationBuilder.RenameIndex(
                name: "IX_DangKyThamGias_MaTrangThai",
                table: "DangKyThamGias",
                newName: "IX_DangKyThamGias_MaTrangThaiDangKy");

            migrationBuilder.AddForeignKey(
                name: "FK_DangKyThamGias_TrangThaiDangKys_MaTrangThaiDangKy",
                table: "DangKyThamGias",
                column: "MaTrangThaiDangKy",
                principalTable: "TrangThaiDangKys",
                principalColumn: "MaTrangThaiDangKy",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DangKyThamGias_TrangThaiDangKys_MaTrangThaiDangKy",
                table: "DangKyThamGias");

            migrationBuilder.RenameColumn(
                name: "TenTrangThaiDangKy",
                table: "TrangThaiDangKys",
                newName: "TenTrangThai");

            migrationBuilder.RenameColumn(
                name: "MaTrangThaiDangKy",
                table: "TrangThaiDangKys",
                newName: "MaTrangThai");

            migrationBuilder.RenameColumn(
                name: "MaTrangThaiDangKy",
                table: "DangKyThamGias",
                newName: "MaTrangThai");

            migrationBuilder.RenameIndex(
                name: "IX_DangKyThamGias_MaTrangThaiDangKy",
                table: "DangKyThamGias",
                newName: "IX_DangKyThamGias_MaTrangThai");

            migrationBuilder.AddForeignKey(
                name: "FK_DangKyThamGias_TrangThaiDangKys_MaTrangThai",
                table: "DangKyThamGias",
                column: "MaTrangThai",
                principalTable: "TrangThaiDangKys",
                principalColumn: "MaTrangThai",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
