using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyHoatDongNgoaiKhoa.Models
{
    public class DanhMucHoatDong
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaDanhMuc { get; set; }
        
        [Required]
        [StringLength(100)]
        public string TenDanhMuc { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? MoTa { get; set; }
        
        [StringLength(50)]
        public string? Icon { get; set; }
        
        public bool TrangThai { get; set; } = true;
        
        // Navigation properties
        public ICollection<HoatDong> DanhSachHoatDong { get; set; } = new List<HoatDong>();
        public ICollection<BanDoDanhMucHoatDong> DanhSachBanDo { get; set; } = new List<BanDoDanhMucHoatDong>();
    }
} 