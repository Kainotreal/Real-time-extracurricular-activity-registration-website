using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyHoatDongNgoaiKhoa.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTo3NF : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Tạo các bảng tham chiếu trước
            migrationBuilder.CreateTable(
                name: "DanhMucHoatDongs",
                columns: table => new
                {
                    MaDanhMuc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDanhMuc = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMucHoatDongs", x => x.MaDanhMuc);
                });

            migrationBuilder.CreateTable(
                name: "DiaDiems",
                columns: table => new
                {
                    MaDiaDiem = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDiaDiem = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ToaNha = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Tang = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Phong = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SoDienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    SoChoNgoi = table.Column<int>(type: "int", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiaDiems", x => x.MaDiaDiem);
                });

            migrationBuilder.CreateTable(
                name: "LoaiHoatDongs",
                columns: table => new
                {
                    MaLoaiHoatDong = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenLoaiHoatDong = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoaiHoatDongs", x => x.MaLoaiHoatDong);
                });

            migrationBuilder.CreateTable(
                name: "Khoas",
                columns: table => new
                {
                    MaKhoa = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenKhoa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MaKhoaVietTat = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TruongKhoa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SoDienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Khoas", x => x.MaKhoa);
                });

            migrationBuilder.CreateTable(
                name: "PhongBans",
                columns: table => new
                {
                    MaPhongBan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenPhongBan = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SoDienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhongBans", x => x.MaPhongBan);
                });

            migrationBuilder.CreateTable(
                name: "ThongBaos",
                columns: table => new
                {
                    MaThongBao = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TieuDe = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    MaHoatDong = table.Column<int>(type: "int", nullable: false),
                    LoaiThongBao = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DaGui = table.Column<bool>(type: "bit", nullable: false),
                    ThoiGianGui = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NguoiGuiId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThongBaos", x => x.MaThongBao);
                });

            migrationBuilder.CreateTable(
                name: "NhatKyThaoTacs",
                columns: table => new
                {
                    MaNhatKy = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaHoatDong = table.Column<int>(type: "int", nullable: false),
                    LoaiThaoTac = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MoTaThaoTac = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NguoiThucHienId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IPAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ThoiGianThucHien = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DuLieuCu = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DuLieuMoi = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhatKyThaoTacs", x => x.MaNhatKy);
                });

            migrationBuilder.CreateTable(
                name: "FileDinhKems",
                columns: table => new
                {
                    MaFile = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaHoatDong = table.Column<int>(type: "int", nullable: false),
                    TenFile = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DuongDan = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LoaiFile = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    KichThuoc = table.Column<long>(type: "bigint", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NguoiUploadId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayUpload = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileDinhKems", x => x.MaFile);
                });

            migrationBuilder.CreateTable(
                name: "LopHocs",
                columns: table => new
                {
                    MaLopHoc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenLopHoc = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaLopVietTat = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    MaKhoa = table.Column<int>(type: "int", nullable: false),
                    GiaoVienChuNhiem = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SiSo = table.Column<int>(type: "int", nullable: false),
                    NamHoc = table.Column<int>(type: "int", nullable: false),
                    HocKy = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LopHocs", x => x.MaLopHoc);
                });

            migrationBuilder.CreateTable(
                name: "BanDoDanhMucHoatDongs",
                columns: table => new
                {
                    MaBanDo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaHoatDong = table.Column<int>(type: "int", nullable: false),
                    MaDanhMuc = table.Column<int>(type: "int", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BanDoDanhMucHoatDongs", x => x.MaBanDo);
                });

            // Thêm dữ liệu mặc định cho các bảng tham chiếu
            migrationBuilder.InsertData(
                table: "DanhMucHoatDongs",
                columns: new[] { "TenDanhMuc", "MoTa", "Icon", "TrangThai" },
                values: new object[,]
                {
                    { "Thể thao", "Các hoạt động thể thao", "fas fa-running", true },
                    { "Văn hóa", "Các hoạt động văn hóa nghệ thuật", "fas fa-music", true },
                    { "Học thuật", "Các hoạt động học thuật, nghiên cứu", "fas fa-graduation-cap", true },
                    { "Tình nguyện", "Các hoạt động tình nguyện xã hội", "fas fa-heart", true },
                    { "Giải trí", "Các hoạt động giải trí", "fas fa-gamepad", true }
                });

            migrationBuilder.InsertData(
                table: "LoaiHoatDongs",
                columns: new[] { "TenLoaiHoatDong", "MoTa", "Icon", "TrangThai" },
                values: new object[,]
                {
                    { "Cá nhân", "Hoạt động dành cho cá nhân", "fas fa-user", true },
                    { "Nhóm nhỏ", "Hoạt động dành cho nhóm nhỏ (2-10 người)", "fas fa-users", true },
                    { "Tập thể", "Hoạt động dành cho tập thể lớn", "fas fa-users", true },
                    { "Toàn trường", "Hoạt động dành cho toàn trường", "fas fa-school", true }
                });

            migrationBuilder.InsertData(
                table: "DiaDiems",
                columns: new[] { "TenDiaDiem", "DiaChi", "SoChoNgoi", "TrangThai" },
                values: new object[,]
                {
                    { "Hội trường A", "Tầng 1, Tòa A", 200, true },
                    { "Sân vận động", "Khu thể thao", 500, true },
                    { "Thư viện", "Tầng 2, Tòa B", 100, true },
                    { "Phòng họp 101", "Tầng 1, Tòa C", 50, true },
                    { "Căng tin", "Tầng trệt, Tòa A", 150, true }
                });

            // Thêm các cột mới vào HoatDongs trước
            migrationBuilder.AddColumn<decimal>(
                name: "ChiPhi",
                table: "HoatDongs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "MaDanhMuc",
                table: "HoatDongs",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "MaDiaDiem",
                table: "HoatDongs",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "MaLoaiHoatDong",
                table: "HoatDongs",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayCapNhat",
                table: "HoatDongs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SoLuongToiDa",
                table: "HoatDongs",
                type: "int",
                nullable: false,
                defaultValue: 50);

            migrationBuilder.AddColumn<string>(
                name: "TrangBiCanThiet",
                table: "HoatDongs",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "YeuCauThamGia",
                table: "HoatDongs",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            // Cập nhật dữ liệu cũ trong HoatDongs (sau khi đã thêm cột)
            migrationBuilder.Sql("UPDATE HoatDongs SET MaDanhMuc = 1, MaLoaiHoatDong = 1, MaDiaDiem = 1, SoLuongToiDa = 50, ChiPhi = 0 WHERE MaDanhMuc = 0 OR MaDanhMuc IS NULL");

            // Tạo indexes
            migrationBuilder.CreateIndex(
                name: "IX_HoatDongs_MaDanhMuc",
                table: "HoatDongs",
                column: "MaDanhMuc");

            migrationBuilder.CreateIndex(
                name: "IX_HoatDongs_MaDiaDiem",
                table: "HoatDongs",
                column: "MaDiaDiem");

            migrationBuilder.CreateIndex(
                name: "IX_HoatDongs_MaLoaiHoatDong",
                table: "HoatDongs",
                column: "MaLoaiHoatDong");

            migrationBuilder.CreateIndex(
                name: "IX_HoatDongs_MaTrangThai",
                table: "HoatDongs",
                column: "MaTrangThai");

            migrationBuilder.CreateIndex(
                name: "IX_DangKyThamGias_MaTrangThai",
                table: "DangKyThamGias",
                column: "MaTrangThai");

            migrationBuilder.CreateIndex(
                name: "IX_BanDoDanhMucHoatDongs_MaDanhMuc",
                table: "BanDoDanhMucHoatDongs",
                column: "MaDanhMuc");

            migrationBuilder.CreateIndex(
                name: "IX_BanDoDanhMucHoatDongs_MaHoatDong",
                table: "BanDoDanhMucHoatDongs",
                column: "MaHoatDong");

            migrationBuilder.CreateIndex(
                name: "IX_FileDinhKems_MaHoatDong",
                table: "FileDinhKems",
                column: "MaHoatDong");

            migrationBuilder.CreateIndex(
                name: "IX_LopHocs_MaKhoa",
                table: "LopHocs",
                column: "MaKhoa");

            migrationBuilder.CreateIndex(
                name: "IX_NhatKyThaoTacs_MaHoatDong",
                table: "NhatKyThaoTacs",
                column: "MaHoatDong");

            migrationBuilder.CreateIndex(
                name: "IX_ThongBaos_MaHoatDong",
                table: "ThongBaos",
                column: "MaHoatDong");

            // Thêm foreign key constraints
            migrationBuilder.AddForeignKey(
                name: "FK_HoatDongs_DanhMucHoatDongs_MaDanhMuc",
                table: "HoatDongs",
                column: "MaDanhMuc",
                principalTable: "DanhMucHoatDongs",
                principalColumn: "MaDanhMuc",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HoatDongs_DiaDiems_MaDiaDiem",
                table: "HoatDongs",
                column: "MaDiaDiem",
                principalTable: "DiaDiems",
                principalColumn: "MaDiaDiem",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HoatDongs_LoaiHoatDongs_MaLoaiHoatDong",
                table: "HoatDongs",
                column: "MaLoaiHoatDong",
                principalTable: "LoaiHoatDongs",
                principalColumn: "MaLoaiHoatDong",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HoatDongs_TrangThaiHoatDongs_MaTrangThai",
                table: "HoatDongs",
                column: "MaTrangThai",
                principalTable: "TrangThaiHoatDongs",
                principalColumn: "MaTrangThai",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DangKyThamGias_TrangThaiDangKys_MaTrangThai",
                table: "DangKyThamGias",
                column: "MaTrangThai",
                principalTable: "TrangThaiDangKys",
                principalColumn: "MaTrangThai",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LopHocs_Khoas_MaKhoa",
                table: "LopHocs",
                column: "MaKhoa",
                principalTable: "Khoas",
                principalColumn: "MaKhoa",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FileDinhKems_HoatDongs_MaHoatDong",
                table: "FileDinhKems",
                column: "MaHoatDong",
                principalTable: "HoatDongs",
                principalColumn: "MaHoatDong",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ThongBaos_HoatDongs_MaHoatDong",
                table: "ThongBaos",
                column: "MaHoatDong",
                principalTable: "HoatDongs",
                principalColumn: "MaHoatDong",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NhatKyThaoTacs_HoatDongs_MaHoatDong",
                table: "NhatKyThaoTacs",
                column: "MaHoatDong",
                principalTable: "HoatDongs",
                principalColumn: "MaHoatDong",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BanDoDanhMucHoatDongs_DanhMucHoatDongs_MaDanhMuc",
                table: "BanDoDanhMucHoatDongs",
                column: "MaDanhMuc",
                principalTable: "DanhMucHoatDongs",
                principalColumn: "MaDanhMuc",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BanDoDanhMucHoatDongs_HoatDongs_MaHoatDong",
                table: "BanDoDanhMucHoatDongs",
                column: "MaHoatDong",
                principalTable: "HoatDongs",
                principalColumn: "MaHoatDong",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Xóa foreign key constraints
            migrationBuilder.DropForeignKey(
                name: "FK_BanDoDanhMucHoatDongs_HoatDongs_MaHoatDong",
                table: "BanDoDanhMucHoatDongs");

            migrationBuilder.DropForeignKey(
                name: "FK_BanDoDanhMucHoatDongs_DanhMucHoatDongs_MaDanhMuc",
                table: "BanDoDanhMucHoatDongs");

            migrationBuilder.DropForeignKey(
                name: "FK_NhatKyThaoTacs_HoatDongs_MaHoatDong",
                table: "NhatKyThaoTacs");

            migrationBuilder.DropForeignKey(
                name: "FK_ThongBaos_HoatDongs_MaHoatDong",
                table: "ThongBaos");

            migrationBuilder.DropForeignKey(
                name: "FK_FileDinhKems_HoatDongs_MaHoatDong",
                table: "FileDinhKems");

            migrationBuilder.DropForeignKey(
                name: "FK_LopHocs_Khoas_MaKhoa",
                table: "LopHocs");

            migrationBuilder.DropForeignKey(
                name: "FK_DangKyThamGias_TrangThaiDangKys_MaTrangThai",
                table: "DangKyThamGias");

            migrationBuilder.DropForeignKey(
                name: "FK_HoatDongs_TrangThaiHoatDongs_MaTrangThai",
                table: "HoatDongs");

            migrationBuilder.DropForeignKey(
                name: "FK_HoatDongs_LoaiHoatDongs_MaLoaiHoatDong",
                table: "HoatDongs");

            migrationBuilder.DropForeignKey(
                name: "FK_HoatDongs_DiaDiems_MaDiaDiem",
                table: "HoatDongs");

            migrationBuilder.DropForeignKey(
                name: "FK_HoatDongs_DanhMucHoatDongs_MaDanhMuc",
                table: "HoatDongs");

            // Xóa indexes
            migrationBuilder.DropIndex(
                name: "IX_ThongBaos_MaHoatDong",
                table: "ThongBaos");

            migrationBuilder.DropIndex(
                name: "IX_NhatKyThaoTacs_MaHoatDong",
                table: "NhatKyThaoTacs");

            migrationBuilder.DropIndex(
                name: "IX_LopHocs_MaKhoa",
                table: "LopHocs");

            migrationBuilder.DropIndex(
                name: "IX_FileDinhKems_MaHoatDong",
                table: "FileDinhKems");

            migrationBuilder.DropIndex(
                name: "IX_BanDoDanhMucHoatDongs_MaHoatDong",
                table: "BanDoDanhMucHoatDongs");

            migrationBuilder.DropIndex(
                name: "IX_BanDoDanhMucHoatDongs_MaDanhMuc",
                table: "BanDoDanhMucHoatDongs");

            migrationBuilder.DropIndex(
                name: "IX_DangKyThamGias_MaTrangThai",
                table: "DangKyThamGias");

            migrationBuilder.DropIndex(
                name: "IX_HoatDongs_MaTrangThai",
                table: "HoatDongs");

            migrationBuilder.DropIndex(
                name: "IX_HoatDongs_MaLoaiHoatDong",
                table: "HoatDongs");

            migrationBuilder.DropIndex(
                name: "IX_HoatDongs_MaDiaDiem",
                table: "HoatDongs");

            migrationBuilder.DropIndex(
                name: "IX_HoatDongs_MaDanhMuc",
                table: "HoatDongs");

            // Xóa các cột mới từ HoatDongs
            migrationBuilder.DropColumn(
                name: "YeuCauThamGia",
                table: "HoatDongs");

            migrationBuilder.DropColumn(
                name: "TrangBiCanThiet",
                table: "HoatDongs");

            migrationBuilder.DropColumn(
                name: "SoLuongToiDa",
                table: "HoatDongs");

            migrationBuilder.DropColumn(
                name: "NgayCapNhat",
                table: "HoatDongs");

            migrationBuilder.DropColumn(
                name: "MaLoaiHoatDong",
                table: "HoatDongs");

            migrationBuilder.DropColumn(
                name: "MaDiaDiem",
                table: "HoatDongs");

            migrationBuilder.DropColumn(
                name: "MaDanhMuc",
                table: "HoatDongs");

            migrationBuilder.DropColumn(
                name: "ChiPhi",
                table: "HoatDongs");

            // Xóa các bảng mới
            migrationBuilder.DropTable(
                name: "BanDoDanhMucHoatDongs");

            migrationBuilder.DropTable(
                name: "FileDinhKems");

            migrationBuilder.DropTable(
                name: "LopHocs");

            migrationBuilder.DropTable(
                name: "NhatKyThaoTacs");

            migrationBuilder.DropTable(
                name: "PhongBans");

            migrationBuilder.DropTable(
                name: "ThongBaos");

            migrationBuilder.DropTable(
                name: "Khoas");

            migrationBuilder.DropTable(
                name: "LoaiHoatDongs");

            migrationBuilder.DropTable(
                name: "DiaDiems");

            migrationBuilder.DropTable(
                name: "DanhMucHoatDongs");
        }
    }
}
