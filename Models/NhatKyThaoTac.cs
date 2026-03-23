using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyHoatDongNgoaiKhoa.Models
{
    public class NhatKyThaoTac
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaNhatKy { get; set; }
        
        [Required]
        public int MaHoatDong { get; set; }
        
        [Required]
        [StringLength(50)]
        public string LoaiThaoTac { get; set; } = string.Empty; // "Tao", "Sua", "Xoa", "DangKy", "HuyDangKy"
        
        [StringLength(500)]
        public string? MoTaThaoTac { get; set; }
        
        public string NguoiThucHienId { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string? IPAddress { get; set; }
        
        [StringLength(200)]
        public string? UserAgent { get; set; }
        
        public DateTime ThoiGianThucHien { get; set; } = DateTime.Now;
        
        [StringLength(1000)]
        public string? DuLieuCu { get; set; }
        
        [StringLength(1000)]
        public string? DuLieuMoi { get; set; }
        
        // Navigation properties
        public HoatDong? HoatDong { get; set; }
    }
} 