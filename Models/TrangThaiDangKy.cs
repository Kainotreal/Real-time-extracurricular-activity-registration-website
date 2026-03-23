using Microsoft.EntityFrameworkCore;
using QuanLyHoatDongNgoaiKhoa.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyHoatDongNgoaiKhoa.Models
{
    public class TrangThaiDangKy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaTrangThaiDangKy { get; set; }
        public string TenTrangThaiDangKy { get; set; } = string.Empty;
        public string? MoTa { get; set; }
        public ICollection<DangKyThamGia> DanhSachDangKy { get; set; } = new List<DangKyThamGia>();
    }
}