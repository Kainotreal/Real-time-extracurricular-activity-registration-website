using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyHoatDongNgoaiKhoa.Models
{
    public class ThongBao
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaThongBao { get; set; }
        
        [Required]
        [StringLength(200)]
        public string TieuDe { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string? NoiDung { get; set; }
        
        [Required]
        public int MaHoatDong { get; set; }
        
        [StringLength(50)]
        public string? LoaiThongBao { get; set; } // "XacNhan", "CapNhat", "Huy", "NhanhChong"
        
        public bool DaGui { get; set; } = false;
        
        public DateTime? ThoiGianGui { get; set; }
        
        public string NguoiGuiId { get; set; } = string.Empty;
        
        public DateTime NgayTao { get; set; } = DateTime.Now;
        
        // Navigation properties
        public HoatDong? HoatDong { get; set; }
    }
} 