using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyHoatDongNgoaiKhoa.Models
{
    public class DiaDiem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaDiaDiem { get; set; }
        
        [Required]
        [StringLength(200)]
        public string TenDiaDiem { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? DiaChi { get; set; }
        
        [StringLength(100)]
        public string? ToaNha { get; set; }
        
        [StringLength(50)]
        public string? Tang { get; set; }
        
        [StringLength(50)]
        public string? Phong { get; set; }
        
        [StringLength(20)]
        public string? SoDienThoai { get; set; }
        
        public int SoChoNgoi { get; set; }
        
        [StringLength(500)]
        public string? MoTa { get; set; }
        
        public bool TrangThai { get; set; } = true;
        
        // Navigation properties
        public ICollection<HoatDong> DanhSachHoatDong { get; set; } = new List<HoatDong>();
    }
} 