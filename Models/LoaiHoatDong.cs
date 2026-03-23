using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyHoatDongNgoaiKhoa.Models
{
    public class LoaiHoatDong
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaLoaiHoatDong { get; set; }
        
        [Required]
        [StringLength(100)]
        public string TenLoaiHoatDong { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? MoTa { get; set; }
        
        [StringLength(50)]
        public string? Icon { get; set; }
        
        public bool TrangThai { get; set; } = true;
        
        // Navigation properties
        public ICollection<HoatDong> DanhSachHoatDong { get; set; } = new List<HoatDong>();
    }
} 