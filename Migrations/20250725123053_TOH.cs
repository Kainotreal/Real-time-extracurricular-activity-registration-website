using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyHoatDongNgoaiKhoa.Migrations
{
    /// <inheritdoc />
    public partial class TOH : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DanhMucHoatDongs",
                columns: table => new
                {
                    MaDanhMuc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDanhMuc = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMucHoatDongs", x => x.MaDanhMuc);
                });

            migrationBuilder.CreateTable(
                name: "TrangThaiDangKys",
                columns: table => new
                {
                    MaTrangThaiDangKy = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenTrangThaiDangKy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrangThaiDangKys", x => x.MaTrangThaiDangKy);
                });

            migrationBuilder.CreateTable(
                name: "TrangThaiHoatDongs",
                columns: table => new
                {
                    MaTrangThai = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenTrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrangThaiHoatDongs", x => x.MaTrangThai);
                });

            migrationBuilder.CreateTable(
                name: "VaiTros",
                columns: table => new
                {
                    MaVaiTro = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenVaiTro = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VaiTros", x => x.MaVaiTro);
                });

            migrationBuilder.CreateTable(
                name: "NguoiDungs",
                columns: table => new
                {
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenNguoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MatKhauHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaVaiTro = table.Column<int>(type: "int", nullable: false),
                    VaiTroMaVaiTro = table.Column<int>(type: "int", nullable: false)
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
                name: "HoatDongs",
                columns: table => new
                {
                    MaHoatDong = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TieuDe = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThoiGianBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThoiGianKetThuc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaTrangThai = table.Column<int>(type: "int", nullable: false),
                    TrangThaiHoatDongMaTrangThai = table.Column<int>(type: "int", nullable: false),
                    NguoiTao = table.Column<int>(type: "int", nullable: false),
                    NguoiDungTaoMaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoatDongs", x => x.MaHoatDong);
                    table.ForeignKey(
                        name: "FK_HoatDongs_NguoiDungs_NguoiDungTaoMaNguoiDung",
                        column: x => x.NguoiDungTaoMaNguoiDung,
                        principalTable: "NguoiDungs",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HoatDongs_TrangThaiHoatDongs_TrangThaiHoatDongMaTrangThai",
                        column: x => x.TrangThaiHoatDongMaTrangThai,
                        principalTable: "TrangThaiHoatDongs",
                        principalColumn: "MaTrangThai",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NhatKyThaoTacs",
                columns: table => new
                {
                    MaNhatKy = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    NguoiDungMaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    HanhDong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChiTiet = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    NguoiDungMaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DaDoc = table.Column<bool>(type: "bit", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "BanDoDanhMucHoatDongs",
                columns: table => new
                {
                    MaBanDo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaHoatDong = table.Column<int>(type: "int", nullable: false),
                    HoatDongMaHoatDong = table.Column<int>(type: "int", nullable: false),
                    MaDanhMuc = table.Column<int>(type: "int", nullable: false),
                    DanhMucHoatDongMaDanhMuc = table.Column<int>(type: "int", nullable: false)
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
                name: "DangKyThamGias",
                columns: table => new
                {
                    MaDangKy = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    MaHoatDong = table.Column<int>(type: "int", nullable: false),
                    MaTrangThaiDangKy = table.Column<int>(type: "int", nullable: false),
                    TrangThaiDangKyMaTrangThaiDangKy = table.Column<int>(type: "int", nullable: false),
                    ThoiGianDangKy = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DangKyThamGias", x => x.MaDangKy);
                    table.ForeignKey(
                        name: "FK_DangKyThamGias_HoatDongs_MaHoatDong",
                        column: x => x.MaHoatDong,
                        principalTable: "HoatDongs",
                        principalColumn: "MaHoatDong",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DangKyThamGias_NguoiDungs_MaNguoiDung",
                        column: x => x.MaNguoiDung,
                        principalTable: "NguoiDungs",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DangKyThamGias_TrangThaiDangKys_TrangThaiDangKyMaTrangThaiDangKy",
                        column: x => x.TrangThaiDangKyMaTrangThaiDangKy,
                        principalTable: "TrangThaiDangKys",
                        principalColumn: "MaTrangThaiDangKy",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BanDoDanhMucHoatDongs_DanhMucHoatDongMaDanhMuc",
                table: "BanDoDanhMucHoatDongs",
                column: "DanhMucHoatDongMaDanhMuc");

            migrationBuilder.CreateIndex(
                name: "IX_BanDoDanhMucHoatDongs_HoatDongMaHoatDong",
                table: "BanDoDanhMucHoatDongs",
                column: "HoatDongMaHoatDong");

            migrationBuilder.CreateIndex(
                name: "IX_DangKyThamGias_MaHoatDong",
                table: "DangKyThamGias",
                column: "MaHoatDong");

            migrationBuilder.CreateIndex(
                name: "IX_DangKyThamGias_MaNguoiDung",
                table: "DangKyThamGias",
                column: "MaNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_DangKyThamGias_TrangThaiDangKyMaTrangThaiDangKy",
                table: "DangKyThamGias",
                column: "TrangThaiDangKyMaTrangThaiDangKy");

            migrationBuilder.CreateIndex(
                name: "IX_HoatDongs_NguoiDungTaoMaNguoiDung",
                table: "HoatDongs",
                column: "NguoiDungTaoMaNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_HoatDongs_TrangThaiHoatDongMaTrangThai",
                table: "HoatDongs",
                column: "TrangThaiHoatDongMaTrangThai");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BanDoDanhMucHoatDongs");

            migrationBuilder.DropTable(
                name: "DangKyThamGias");

            migrationBuilder.DropTable(
                name: "NhatKyThaoTacs");

            migrationBuilder.DropTable(
                name: "ThongBaos");

            migrationBuilder.DropTable(
                name: "DanhMucHoatDongs");

            migrationBuilder.DropTable(
                name: "HoatDongs");

            migrationBuilder.DropTable(
                name: "TrangThaiDangKys");

            migrationBuilder.DropTable(
                name: "NguoiDungs");

            migrationBuilder.DropTable(
                name: "TrangThaiHoatDongs");

            migrationBuilder.DropTable(
                name: "VaiTros");
        }
    }
}
