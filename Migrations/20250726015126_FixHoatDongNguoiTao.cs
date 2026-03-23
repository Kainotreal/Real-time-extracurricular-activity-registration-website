using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyHoatDongNgoaiKhoa.Migrations
{
    /// <inheritdoc />
    public partial class FixHoatDongNguoiTao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HoatDongs_TrangThaiHoatDongs_TrangThaiHoatDongMaTrangThai",
                table: "HoatDongs");

            migrationBuilder.DropColumn(
                name: "NguoiTao",
                table: "HoatDongs");

            migrationBuilder.AlterColumn<int>(
                name: "TrangThaiHoatDongMaTrangThai",
                table: "HoatDongs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "TieuDe",
                table: "HoatDongs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "MoTa",
                table: "HoatDongs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_HoatDongs_TrangThaiHoatDongs_TrangThaiHoatDongMaTrangThai",
                table: "HoatDongs",
                column: "TrangThaiHoatDongMaTrangThai",
                principalTable: "TrangThaiHoatDongs",
                principalColumn: "MaTrangThai");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HoatDongs_TrangThaiHoatDongs_TrangThaiHoatDongMaTrangThai",
                table: "HoatDongs");

            migrationBuilder.AlterColumn<int>(
                name: "TrangThaiHoatDongMaTrangThai",
                table: "HoatDongs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TieuDe",
                table: "HoatDongs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MoTa",
                table: "HoatDongs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NguoiTao",
                table: "HoatDongs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_HoatDongs_TrangThaiHoatDongs_TrangThaiHoatDongMaTrangThai",
                table: "HoatDongs",
                column: "TrangThaiHoatDongMaTrangThai",
                principalTable: "TrangThaiHoatDongs",
                principalColumn: "MaTrangThai",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
