using System.ComponentModel.DataAnnotations;

namespace QuanLyHoatDongNgoaiKhoa.ViewModels
{
    public class EditHoatDongViewModel
    {
        public int MaHoatDong { get; set; }
        
        [Required(ErrorMessage = "Tiêu đề không được để trống!")]
        [StringLength(200, ErrorMessage = "Tiêu đề không được quá 200 ký tự!")]
        public string TieuDe { get; set; } = string.Empty;
        
        [StringLength(1000, ErrorMessage = "Mô tả không được quá 1000 ký tự!")]
        public string? MoTa { get; set; }
        
        [Required(ErrorMessage = "Thời gian bắt đầu không được để trống!")]
        public DateTime ThoiGianBatDau { get; set; }
        
        [Required(ErrorMessage = "Thời gian kết thúc không được để trống!")]
        public DateTime ThoiGianKetThuc { get; set; }
        
        [Required(ErrorMessage = "Vui lòng chọn trạng thái!")]
        public int MaTrangThai { get; set; }
        
        // Các field hiển thị thông tin (không edit)
        public string NguoiTaoId { get; set; } = string.Empty;
        public DateTime NgayTao { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        public int MaDanhMuc { get; set; }
        public int MaLoaiHoatDong { get; set; }
        public int MaDiaDiem { get; set; }
        public int SoLuongToiDa { get; set; }
        public decimal ChiPhi { get; set; }
        public string? YeuCauThamGia { get; set; }
        public string? TrangBiCanThiet { get; set; }
        
        // Navigation properties cho hiển thị
        public string? TenTrangThai { get; set; }
        public string? TenDanhMuc { get; set; }
        public string? TenLoaiHoatDong { get; set; }
        public string? TenDiaDiem { get; set; }
    }
} 