using Microsoft.EntityFrameworkCore;
using QuanLyHoatDongNgoaiKhoa.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyHoatDongNgoaiKhoa.Models
{
    public class TrangThaiHoatDong
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaTrangThai { get; set; }
        public string TenTrangThai { get; set; } = string.Empty;
        public string? MoTa { get; set; }
        public ICollection<HoatDong> DanhSachHoatDong { get; set; } = new List<HoatDong>();
    }
}