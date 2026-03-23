namespace QuanLyHoatDongNgoaiKhoa.ViewModels
{
    public class ErrorViewModel
    {
        public string? Message { get; set; }
        public string? Details { get; set; }
        public bool ShowDetails { get; set; } = false;
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
} 