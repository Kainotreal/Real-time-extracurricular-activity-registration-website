using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyHoatDongNgoaiKhoa.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DangKyThamGias_TrangThaiDangKys_TrangThaiDangKyMaTrangThaiDangKy",
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
                name: "NgayTao",
                table: "ThongBaos",
                newName: "ThoiGianGui");

            migrationBuilder.RenameColumn(
                name: "NgayTao",
                table: "NhatKyThaoTacs",
                newName: "ThoiGianThaoTac");

            migrationBuilder.RenameColumn(
                name: "TenNguoiDung",
                table: "NguoiDungs",
                newName: "HoTen");

            migrationBuilder.RenameColumn(
                name: "TrangThaiDangKyMaTrangThaiDangKy",
                table: "DangKyThamGias",
                newName: "TrangThaiDangKyMaTrangThai");

            migrationBuilder.RenameColumn(
                name: "ThoiGianDangKy",
                table: "DangKyThamGias",
                newName: "NgayDangKy");

            migrationBuilder.RenameColumn(
                name: "MaTrangThaiDangKy",
                table: "DangKyThamGias",
                newName: "MaTrangThai");

            migrationBuilder.RenameIndex(
                name: "IX_DangKyThamGias_TrangThaiDangKyMaTrangThaiDangKy",
                table: "DangKyThamGias",
                newName: "IX_DangKyThamGias_TrangThaiDangKyMaTrangThai");

            migrationBuilder.AddColumn<string>(
                name: "MoTa",
                table: "VaiTros",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MoTa",
                table: "TrangThaiHoatDongs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MoTa",
                table: "TrangThaiDangKys",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayTao",
                table: "NguoiDungs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "SoDienThoai",
                table: "NguoiDungs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MoTa",
                table: "DanhMucHoatDongs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GhiChu",
                table: "DangKyThamGias",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DangKyThamGias_TrangThaiDangKys_TrangThaiDangKyMaTrangThai",
                table: "DangKyThamGias",
                column: "TrangThaiDangKyMaTrangThai",
                principalTable: "TrangThaiDangKys",
                principalColumn: "MaTrangThai",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DangKyThamGias_TrangThaiDangKys_TrangThaiDangKyMaTrangThai",
                table: "DangKyThamGias");

            migrationBuilder.DropColumn(
                name: "MoTa",
                table: "VaiTros");

            migrationBuilder.DropColumn(
                name: "MoTa",
                table: "TrangThaiHoatDongs");

            migrationBuilder.DropColumn(
                name: "MoTa",
                table: "TrangThaiDangKys");

            migrationBuilder.DropColumn(
                name: "NgayTao",
                table: "NguoiDungs");

            migrationBuilder.DropColumn(
                name: "SoDienThoai",
                table: "NguoiDungs");

            migrationBuilder.DropColumn(
                name: "MoTa",
                table: "DanhMucHoatDongs");

            migrationBuilder.DropColumn(
                name: "GhiChu",
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
                name: "ThoiGianGui",
                table: "ThongBaos",
                newName: "NgayTao");

            migrationBuilder.RenameColumn(
                name: "ThoiGianThaoTac",
                table: "NhatKyThaoTacs",
                newName: "NgayTao");

            migrationBuilder.RenameColumn(
                name: "HoTen",
                table: "NguoiDungs",
                newName: "TenNguoiDung");

            migrationBuilder.RenameColumn(
                name: "TrangThaiDangKyMaTrangThai",
                table: "DangKyThamGias",
                newName: "TrangThaiDangKyMaTrangThaiDangKy");

            migrationBuilder.RenameColumn(
                name: "NgayDangKy",
                table: "DangKyThamGias",
                newName: "ThoiGianDangKy");

            migrationBuilder.RenameColumn(
                name: "MaTrangThai",
                table: "DangKyThamGias",
                newName: "MaTrangThaiDangKy");

            migrationBuilder.RenameIndex(
                name: "IX_DangKyThamGias_TrangThaiDangKyMaTrangThai",
                table: "DangKyThamGias",
                newName: "IX_DangKyThamGias_TrangThaiDangKyMaTrangThaiDangKy");

            migrationBuilder.AddForeignKey(
                name: "FK_DangKyThamGias_TrangThaiDangKys_TrangThaiDangKyMaTrangThaiDangKy",
                table: "DangKyThamGias",
                column: "TrangThaiDangKyMaTrangThaiDangKy",
                principalTable: "TrangThaiDangKys",
                principalColumn: "MaTrangThaiDangKy",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
