using Microsoft.AspNetCore.SignalR;
using QuanLyHoatDongNgoaiKhoa.Hubs;
using QuanLyHoatDongNgoaiKhoa.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace QuanLyHoatDongNgoaiKhoa.Services
{
    public interface INotificationService
    {
        Task SendActivityCreatedNotification(HoatDong hoatDong, string createdBy);
        Task SendRegistrationNotification(DangKyThamGia dangKy, HoatDong hoatDong, string studentName);
        Task SendRegistrationConfirmationNotification(DangKyThamGia dangKy, HoatDong hoatDong, string status);
        Task SendActivityConfirmationNotification(HoatDong hoatDong, string status);
        Task SendCustomNotification(string message, string type, string title, string targetGroup = "all");
    }

    public class NotificationService : INotificationService
    {
        private readonly IHubContext<HubThongBao> _hubContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppDbContext _context;

        public NotificationService(IHubContext<HubThongBao> hubContext, UserManager<IdentityUser> userManager, AppDbContext context)
        {
            _hubContext = hubContext;
            _userManager = userManager;
            _context = context;
        }

        public async Task SendActivityCreatedNotification(HoatDong hoatDong, string createdBy)
        {
            // Lưu thông báo vào database cho tất cả Admin
            var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
            foreach (var admin in adminUsers)
            {
                var thongBaoAdmin = new ThongBao
                {
                    TieuDe = "Hoạt động mới cần xác nhận",
                    NoiDung = $"Hoạt động mới: {hoatDong.TieuDe} được tạo bởi {createdBy}",
                    MaHoatDong = hoatDong.MaHoatDong,
                    LoaiThongBao = "XacNhan",
                    NguoiGuiId = admin.Id, // Lưu cho từng Admin
                    NgayTao = DateTime.Now
                };
                _context.ThongBaos.Add(thongBaoAdmin);
            }
            await _context.SaveChangesAsync();

            // Gửi thông báo realtime cho tất cả Admin
            var notification = new
            {
                message = $"Hoạt động mới: {hoatDong.TieuDe} cần xác nhận",
                type = "warning",
                title = "Hoạt động mới cần xác nhận",
                timestamp = DateTime.Now.ToString("HH:mm:ss"),
                id = Guid.NewGuid().ToString()
            };
            await _hubContext.Clients.Group("Admin").SendAsync("ReceiveNotification", notification);
        }

        public async Task SendRegistrationNotification(DangKyThamGia dangKy, HoatDong hoatDong, string studentName)
        {
            var message = $"Sinh viên {studentName} đã đăng ký tham gia hoạt động: {hoatDong.TieuDe}";

            // Lưu thông báo vào database cho tất cả Teacher
            var teacherUsers = await _userManager.GetUsersInRoleAsync("Teacher");
            foreach (var teacher in teacherUsers)
            {
                var thongBaoTeacher = new ThongBao
                {
                    TieuDe = "Đăng ký mới cần xác nhận",
                    NoiDung = message,
                    MaHoatDong = hoatDong.MaHoatDong,
                    LoaiThongBao = "XacNhan",
                    NguoiGuiId = teacher.Id, // Lưu cho từng Teacher
                    NgayTao = DateTime.Now
                };
                _context.ThongBaos.Add(thongBaoTeacher);
            }
            await _context.SaveChangesAsync();

            // Gửi thông báo realtime cho Teacher
            var teacherNotification = new
            {
                message = message,
                type = "info",
                title = "Đăng ký mới cần xác nhận",
                timestamp = DateTime.Now.ToString("HH:mm:ss"),
                id = Guid.NewGuid().ToString()
            };
            await _hubContext.Clients.Group("Teacher").SendAsync("ReceiveNotification", teacherNotification);

            // Gửi thông báo realtime cho Admin
            var adminNotification = new
            {
                message = message,
                type = "info",
                title = "Đăng ký mới",
                timestamp = DateTime.Now.ToString("HH:mm:ss"),
                id = Guid.NewGuid().ToString()
            };
            await _hubContext.Clients.Group("Admin").SendAsync("ReceiveNotification", adminNotification);
        }

        public async Task SendRegistrationConfirmationNotification(DangKyThamGia dangKy, HoatDong hoatDong, string status)
        {
            var user = await _userManager.FindByIdAsync(dangKy.UserId);
            var studentName = user?.UserName ?? "Sinh viên";

            var message = $"Đăng ký tham gia hoạt động {hoatDong.TieuDe} của {studentName} đã được {status}";

            // Lưu thông báo vào database cho Student
            var thongBaoStudent = new ThongBao
            {
                TieuDe = "Xác nhận đăng ký",
                NoiDung = message,
                MaHoatDong = hoatDong.MaHoatDong,
                LoaiThongBao = status == "xác nhận" ? "XacNhan" : "TuChoi",
                NguoiGuiId = dangKy.UserId,
                NgayTao = DateTime.Now
            };
            _context.ThongBaos.Add(thongBaoStudent);
            await _context.SaveChangesAsync();

            // Gửi thông báo realtime cho Student
            var studentNotification = new
            {
                message = message,
                type = "success",
                title = "Xác nhận đăng ký",
                timestamp = DateTime.Now.ToString("HH:mm:ss"),
                id = Guid.NewGuid().ToString()
            };
            await _hubContext.Clients.Group($"User_{dangKy.UserId}").SendAsync("ReceiveNotification", studentNotification);
        }

        public async Task SendActivityConfirmationNotification(HoatDong hoatDong, string status)
        {
            var message = $"Hoạt động {hoatDong.TieuDe} đã được {status}";

            // Lưu thông báo vào database cho tất cả user
            var allUsers = await _userManager.Users.ToListAsync();
            foreach (var user in allUsers)
            {
                var thongBao = new ThongBao
                {
                    TieuDe = "Xác nhận hoạt động",
                    NoiDung = message,
                    MaHoatDong = hoatDong.MaHoatDong,
                    LoaiThongBao = status == "xác nhận" ? "XacNhan" : "TuChoi",
                    NguoiGuiId = user.Id, // Lưu cho từng user
                    NgayTao = DateTime.Now
                };
                _context.ThongBaos.Add(thongBao);
            }
            await _context.SaveChangesAsync();

            // Gửi thông báo realtime cho tất cả
            var allNotification = new
            {
                message = message,
                type = "success",
                title = "Xác nhận hoạt động",
                timestamp = DateTime.Now.ToString("HH:mm:ss"),
                id = Guid.NewGuid().ToString()
            };
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", allNotification);
        }

        public async Task SendCustomNotification(string message, string type, string title, string targetGroup = "all")
        {
            var notification = new
            {
                message = message,
                type = type,
                title = title,
                timestamp = DateTime.Now.ToString("HH:mm:ss"),
                id = Guid.NewGuid().ToString()
            };

            switch (targetGroup.ToLower())
            {
                case "admin":
                    await _hubContext.Clients.Group("Admin").SendAsync("ReceiveNotification", notification);
                    break;
                case "teacher":
                    await _hubContext.Clients.Group("Teacher").SendAsync("ReceiveNotification", notification);
                    break;
                case "student":
                    await _hubContext.Clients.Group("Student").SendAsync("ReceiveNotification", notification);
                    break;
                default:
                    await _hubContext.Clients.All.SendAsync("ReceiveNotification", notification);
                    break;
            }
        }
    }
} 