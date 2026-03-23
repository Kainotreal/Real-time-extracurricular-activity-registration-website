using Microsoft.EntityFrameworkCore;
using QuanLyHoatDongNgoaiKhoa.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace QuanLyHoatDongNgoaiKhoa.Models
{
    public class DangKyThamGia
    {
        [Key]
        public int MaDangKy { get; set; }
        public string UserId { get; set; } = string.Empty; // AspNetUsers Id
        public IdentityUser? User { get; set; } // Navigation property to AspNetUsers
        public int MaHoatDong { get; set; }
        public HoatDong? HoatDong { get; set; }
        public int MaTrangThaiDangKy { get; set; }
        public TrangThaiDangKy? TrangThaiDangKy { get; set; }
        public DateTime NgayDangKy { get; set; }
        public string? GhiChu { get; set; }
    }
}