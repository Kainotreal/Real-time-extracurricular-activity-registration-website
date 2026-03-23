using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyHoatDongNgoaiKhoa.Models
{
    public class BanDoDanhMucHoatDong
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaBanDo { get; set; }
        
        [Required]
        public int MaHoatDong { get; set; }
        
        [Required]
        public int MaDanhMuc { get; set; }
        
        public DateTime NgayTao { get; set; } = DateTime.Now;
        
        // Navigation properties
        public HoatDong? HoatDong { get; set; }
        public DanhMucHoatDong? DanhMucHoatDong { get; set; }
    }
} 