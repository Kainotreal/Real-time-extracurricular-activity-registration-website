using Microsoft.EntityFrameworkCore;
using QuanLyHoatDongNgoaiKhoa.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace QuanLyHoatDongNgoaiKhoa.Models
{
    public class HoatDong
    {
        [Key]
        public int MaHoatDong { get; set; }
        
        [Required]
        [StringLength(200)]
        public string TieuDe { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string? MoTa { get; set; }
        
        public DateTime ThoiGianBatDau { get; set; }
        public DateTime ThoiGianKetThuc { get; set; }
        
        [Required]
        public int MaTrangThai { get; set; }
        
        [Required]
        public int MaDanhMuc { get; set; }
        
        [Required]
        public int MaLoaiHoatDong { get; set; }
        
        [Required]
        public int MaDiaDiem { get; set; }
        
        [Required]
        public int SoLuongToiDa { get; set; }
        
        [Required]
        public decimal ChiPhi { get; set; }
        
        [StringLength(500)]
        public string? YeuCauThamGia { get; set; }
        
        [StringLength(500)]
        public string? TrangBiCanThiet { get; set; }
        
        public string NguoiTaoId { get; set; } = string.Empty; // Identity User ID
        public DateTime NgayTao { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        
        // Navigation properties
        public TrangThaiHoatDong? TrangThaiHoatDong { get; set; }
        public DanhMucHoatDong? DanhMucHoatDong { get; set; }
        public LoaiHoatDong? LoaiHoatDong { get; set; }
        public DiaDiem? DiaDiem { get; set; }
        public ICollection<DangKyThamGia>? DanhSachDangKy { get; set; }
        public ICollection<FileDinhKem>? DanhSachFile { get; set; }
        public ICollection<ThongBao>? DanhSachThongBao { get; set; }
        public ICollection<NhatKyThaoTac>? DanhSachNhatKy { get; set; }
        public ICollection<BanDoDanhMucHoatDong>? DanhSachBanDo { get; set; }
        public IdentityUser? NguoiTao { get; set; }
    }
}