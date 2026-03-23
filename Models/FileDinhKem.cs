using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyHoatDongNgoaiKhoa.Models
{
    public class FileDinhKem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaFile { get; set; }
        
        [Required]
        public int MaHoatDong { get; set; }
        
        [Required]
        [StringLength(200)]
        public string TenFile { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? DuongDan { get; set; }
        
        [StringLength(50)]
        public string? LoaiFile { get; set; } // "Image", "Document", "Video", "Audio"
        
        public long KichThuoc { get; set; }
        
        [StringLength(100)]
        public string? ContentType { get; set; }
        
        public string NguoiUploadId { get; set; } = string.Empty;
        
        public DateTime NgayUpload { get; set; } = DateTime.Now;
        
        public bool TrangThai { get; set; } = true;
        
        // Navigation properties
        public HoatDong? HoatDong { get; set; }
    }
} 