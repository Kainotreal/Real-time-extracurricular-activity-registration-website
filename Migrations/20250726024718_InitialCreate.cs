using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyHoatDongNgoaiKhoa.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DangKyThamGias_NguoiDungs_MaNguoiDung",
                table: "DangKyThamGias");

            migrationBuilder.DropForeignKey(
                name: "FK_HoatDongs_NguoiDungs_NguoiDungTaoMaNguoiDung",
                table: "HoatDongs");

            migrationBuilder.DropTable(
                name: "BanDoDanhMucHoatDongs");

            migrationBuilder.DropTable(
                name: "NhatKyThaoTacs");

            migrationBuilder.DropTable(
                name: "ThongBaos");

            migrationBuilder.DropTable(
                name: "DanhMucHoatDongs");

            migrationBuilder.DropTable(
                name: "NguoiDungs");

            migrationBuilder.DropTable(
                name: "VaiTros");

            migrationBuilder.DropIndex(
                name: "IX_HoatDongs_NguoiDungTaoMaNguoiDung",
                table: "HoatDongs");

            migrationBuilder.DropIndex(
                name: "IX_DangKyThamGias_MaNguoiDung",
                table: "DangKyThamGias");

            migrationBuilder.DropColumn(
                name: "NguoiDungTaoMaNguoiDung",
                table: "HoatDongs");

            migrationBuilder.DropColumn(
                name: "MaNguoiDung",
                table: "DangKyThamGias");

            migrationBuilder.AddColumn<string>(
                name: "NguoiTaoId",
                table: "HoatDongs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NguoiDungId",
                table: "DangKyThamGias",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NguoiTaoId",
                table: "HoatDongs");

            migrationBuilder.DropColumn(
                name: "NguoiDungId",
                table: "DangKyThamGias");

            migrationBuilder.AddColumn<int>(
                name: "NguoiDungTaoMaNguoiDung",
                table: "HoatDongs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaNguoiDung",
                table: "DangKyThamGias",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "DanhMucHoatDongs",
                columns: table => new
                {
                    MaDanhMuc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenDanhMuc = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMucHoatDongs", x => x.MaDanhMuc);
                });

            migrationBuilder.CreateTable(
                name: "VaiTros",
                columns: table => new
                {
                    MaVaiTro = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenVaiTro = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VaiTros", x => x.MaVaiTro);
                });

            migrationBuilder.CreateTable(
                name: "BanDoDanhMucHoatDongs",
                columns: table => new
                {
                    MaBanDo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DanhMucHoatDongMaDanhMuc = table.Column<int>(type: "int", nullable: false),
                    HoatDongMaHoatDong = table.Column<int>(type: "int", nullable: false),
                    MaDanhMuc = table.Column<int>(type: "int", nullable: false),
                    MaHoatDong = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BanDoDanhMucHoatDongs", x => x.MaBanDo);
                    table.ForeignKey(
                        name: "FK_BanDoDanhMucHoatDongs_DanhMucHoatDongs_DanhMucHoatDongMaDanhMuc",
                        column: x => x.DanhMucHoatDongMaDanhMuc,
                        principalTable: "DanhMucHoatDongs",
                        principalColumn: "MaDanhMuc",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BanDoDanhMucHoatDongs_HoatDongs_HoatDongMaHoatDong",
                        column: x => x.HoatDongMaHoatDong,
                        principalTable: "HoatDongs",
                        principalColumn: "MaHoatDong",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NguoiDungs",
                columns: table => new
                {
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VaiTroMaVaiTro = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaVaiTro = table.Column<int>(type: "int", nullable: false),
                    MatKhauHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NguoiDungs", x => x.MaNguoiDung);
                    table.ForeignKey(
                        name: "FK_NguoiDungs_VaiTros_VaiTroMaVaiTro",
                        column: x => x.VaiTroMaVaiTro,
                        principalTable: "VaiTros",
                        principalColumn: "MaVaiTro",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NhatKyThaoTacs",
                columns: table => new
                {
                    MaNhatKy = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NguoiDungMaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    ChiTiet = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HanhDong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    ThoiGianThaoTac = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhatKyThaoTacs", x => x.MaNhatKy);
                    table.ForeignKey(
                        name: "FK_NhatKyThaoTacs_NguoiDungs_NguoiDungMaNguoiDung",
                        column: x => x.NguoiDungMaNguoiDung,
                        principalTable: "NguoiDungs",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThongBaos",
                columns: table => new
                {
                    MaThongBao = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NguoiDungMaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    DaDoc = table.Column<bool>(type: "bit", nullable: false),
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThoiGianGui = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThongBaos", x => x.MaThongBao);
                    table.ForeignKey(
                        name: "FK_ThongBaos_NguoiDungs_NguoiDungMaNguoiDung",
                        column: x => x.NguoiDungMaNguoiDung,
                        principalTable: "NguoiDungs",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HoatDongs_NguoiDungTaoMaNguoiDung",
                table: "HoatDongs",
                column: "NguoiDungTaoMaNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_DangKyThamGias_MaNguoiDung",
                table: "DangKyThamGias",
                column: "MaNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_BanDoDanhMucHoatDongs_DanhMucHoatDongMaDanhMuc",
                table: "BanDoDanhMucHoatDongs",
                column: "DanhMucHoatDongMaDanhMuc");

            migrationBuilder.CreateIndex(
                name: "IX_BanDoDanhMucHoatDongs_HoatDongMaHoatDong",
                table: "BanDoDanhMucHoatDongs",
                column: "HoatDongMaHoatDong");

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDungs_VaiTroMaVaiTro",
                table: "NguoiDungs",
                column: "VaiTroMaVaiTro");

            migrationBuilder.CreateIndex(
                name: "IX_NhatKyThaoTacs_NguoiDungMaNguoiDung",
                table: "NhatKyThaoTacs",
                column: "NguoiDungMaNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_ThongBaos_NguoiDungMaNguoiDung",
                table: "ThongBaos",
                column: "NguoiDungMaNguoiDung");

            migrationBuilder.AddForeignKey(
                name: "FK_DangKyThamGias_NguoiDungs_MaNguoiDung",
                table: "DangKyThamGias",
                column: "MaNguoiDung",
                principalTable: "NguoiDungs",
                principalColumn: "MaNguoiDung",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HoatDongs_NguoiDungs_NguoiDungTaoMaNguoiDung",
                table: "HoatDongs",
                column: "NguoiDungTaoMaNguoiDung",
                principalTable: "NguoiDungs",
                principalColumn: "MaNguoiDung",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
