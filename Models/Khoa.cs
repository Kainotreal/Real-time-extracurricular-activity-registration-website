using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyHoatDongNgoaiKhoa.Models
{
    public class Khoa
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaKhoa { get; set; }
        
        [Required]
        [StringLength(100)]
        public string TenKhoa { get; set; } = string.Empty;
        
        [StringLength(10)]
        public string? MaKhoaVietTat { get; set; }
        
        [StringLength(500)]
        public string? MoTa { get; set; }
        
        [StringLength(100)]
        public string? TruongKhoa { get; set; }
        
        [StringLength(20)]
        public string? SoDienThoai { get; set; }
        
        [StringLength(100)]
        public string? Email { get; set; }
        
        public bool TrangThai { get; set; } = true;
        
        public DateTime NgayTao { get; set; } = DateTime.Now;
        
        // Navigation properties
        public ICollection<LopHoc> DanhSachLopHoc { get; set; } = new List<LopHoc>();
    }
} 