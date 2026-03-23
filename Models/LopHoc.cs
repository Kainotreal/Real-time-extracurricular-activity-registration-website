using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyHoatDongNgoaiKhoa.Models
{
    public class LopHoc
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaLopHoc { get; set; }
        
        [Required]
        [StringLength(50)]
        public string TenLopHoc { get; set; } = string.Empty;
        
        [StringLength(10)]
        public string? MaLopVietTat { get; set; }
        
        [Required]
        public int MaKhoa { get; set; }
        
        [StringLength(100)]
        public string? GiaoVienChuNhiem { get; set; }
        
        public int SiSo { get; set; }
        
        public int NamHoc { get; set; }
        
        [StringLength(20)]
        public string? HocKy { get; set; }
        
        [StringLength(500)]
        public string? MoTa { get; set; }
        
        public bool TrangThai { get; set; } = true;
        
        public DateTime NgayTao { get; set; } = DateTime.Now;
        
        // Navigation properties
        public Khoa? Khoa { get; set; }
    }
} 