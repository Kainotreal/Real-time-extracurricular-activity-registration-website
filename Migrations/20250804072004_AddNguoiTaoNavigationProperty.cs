using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyHoatDongNgoaiKhoa.Migrations
{
    /// <inheritdoc />
    public partial class AddNguoiTaoNavigationProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NguoiTaoId",
                table: "HoatDongs",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_HoatDongs_NguoiTaoId",
                table: "HoatDongs",
                column: "NguoiTaoId");

            migrationBuilder.AddForeignKey(
                name: "FK_HoatDongs_AspNetUsers_NguoiTaoId",
                table: "HoatDongs",
                column: "NguoiTaoId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HoatDongs_AspNetUsers_NguoiTaoId",
                table: "HoatDongs");

            migrationBuilder.DropIndex(
                name: "IX_HoatDongs_NguoiTaoId",
                table: "HoatDongs");

            migrationBuilder.AlterColumn<string>(
                name: "NguoiTaoId",
                table: "HoatDongs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
