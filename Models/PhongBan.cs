using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyHoatDongNgoaiKhoa.Models
{
    public class PhongBan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaPhongBan { get; set; }
        
        [Required]
        [StringLength(100)]
        public string TenPhongBan { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? MoTa { get; set; }
        
        [StringLength(20)]
        public string? SoDienThoai { get; set; }
        
        [StringLength(100)]
        public string? Email { get; set; }
        
        [StringLength(200)]
        public string? DiaChi { get; set; }
        
        public bool TrangThai { get; set; } = true;
        
        public DateTime NgayTao { get; set; } = DateTime.Now;
    }
} 