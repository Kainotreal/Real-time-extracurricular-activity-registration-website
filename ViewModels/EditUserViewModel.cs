using System.ComponentModel.DataAnnotations;

namespace QuanLyHoatDongNgoaiKhoa.ViewModels
{
    public class EditUserViewModel
    {
        public string Id { get; set; } = string.Empty;
        
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
        
        [Display(Name = "Tên đăng nhập")]
        public string UserName { get; set; } = string.Empty;
        
        public List<string> CurrentRoles { get; set; } = new List<string>();
        public List<string> AvailableRoles { get; set; } = new List<string>();
        
        [Display(Name = "Vai trò")]
        public List<string> SelectedRoles { get; set; } = new List<string>();
    }
} 