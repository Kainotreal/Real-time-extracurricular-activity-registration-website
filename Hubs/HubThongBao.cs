using Microsoft.AspNetCore.SignalR;
using QuanLyHoatDongNgoaiKhoa.Models;
using System.Text.Json;

namespace QuanLyHoatDongNgoaiKhoa.Hubs
{
    public class HubThongBao : Hub
    {
        private readonly AppDbContext _context;

        public HubThongBao(AppDbContext context)
        {
            _context = context;
        }

        public async Task JoinAdminGroup()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Admin");
        }

        public async Task JoinUserGroup(string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
        }

        public async Task JoinTeacherGroup()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Teacher");
        }

        public async Task JoinStudentGroup()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Student");
        }

        // Gửi thông báo đến tất cả người dùng
        public async Task SendNotificationToAll(string message, string type = "info", string title = "Thông báo")
        {
            var notification = new
            {
                message = message,
                type = type,
                title = title,
                timestamp = DateTime.Now.ToString("HH:mm:ss"),
                id = Guid.NewGuid().ToString()
            };
            await Clients.All.SendAsync("ReceiveNotification", JsonSerializer.Serialize(notification));
        }

        // Gửi thông báo đến Admin
        public async Task SendNotificationToAdmin(string message, string type = "info", string title = "Thông báo Admin")
        {
            var notification = new
            {
                message = message,
                type = type,
                title = title,
                timestamp = DateTime.Now.ToString("HH:mm:ss"),
                id = Guid.NewGuid().ToString()
            };
            await Clients.Group("Admin").SendAsync("ReceiveNotification", JsonSerializer.Serialize(notification));
        }

        // Gửi thông báo đến Teacher
        public async Task SendNotificationToTeacher(string message, string type = "info", string title = "Thông báo Giáo viên")
        {
            var notification = new
            {
                message = message,
                type = type,
                title = title,
                timestamp = DateTime.Now.ToString("HH:mm:ss"),
                id = Guid.NewGuid().ToString()
            };
            await Clients.Group("Teacher").SendAsync("ReceiveNotification", JsonSerializer.Serialize(notification));
        }

        // Gửi thông báo đến Student
        public async Task SendNotificationToStudent(string message, string type = "info", string title = "Thông báo Sinh viên")
        {
            var notification = new
            {
                message = message,
                type = type,
                title = title,
                timestamp = DateTime.Now.ToString("HH:mm:ss"),
                id = Guid.NewGuid().ToString()
            };
            await Clients.Group("Student").SendAsync("ReceiveNotification", JsonSerializer.Serialize(notification));
        }

        // Gửi thông báo đến người dùng cụ thể
        public async Task SendNotificationToUser(string userId, string message, string type = "info", string title = "Thông báo")
        {
            var notification = new
            {
                message = message,
                type = type,
                title = title,
                timestamp = DateTime.Now.ToString("HH:mm:ss"),
                id = Guid.NewGuid().ToString()
            };
            await Clients.Group($"User_{userId}").SendAsync("ReceiveNotification", JsonSerializer.Serialize(notification));
        }

        // Thông báo hoạt động mới
        public async Task SendActivityNotification(string activityName, string createdBy)
        {
            var message = $"Hoạt động mới: {activityName} được tạo bởi {createdBy}";
            await SendNotificationToAll(message, "success", "Hoạt động mới");
            await SendNotificationToAdmin(message, "success", "Hoạt động mới cần xác nhận");
        }

        // Thông báo đăng ký mới
        public async Task SendRegistrationNotification(string activityName, string studentName)
        {
            var message = $"Sinh viên {studentName} đã đăng ký tham gia hoạt động: {activityName}";
            await SendNotificationToTeacher(message, "info", "Đăng ký mới cần xác nhận");
            await SendNotificationToAdmin(message, "info", "Đăng ký mới");
        }

        // Thông báo xác nhận đăng ký
        public async Task SendConfirmationNotification(string activityName, string studentName, string status)
        {
            var message = $"Đăng ký tham gia hoạt động {activityName} của {studentName} đã được {status}";
            await SendNotificationToUser(studentName, message, "success", "Xác nhận đăng ký");
        }

        // Thông báo xác nhận hoạt động
        public async Task SendActivityConfirmationNotification(string activityName, string status)
        {
            var message = $"Hoạt động {activityName} đã được {status}";
            await SendNotificationToAll(message, "success", "Xác nhận hoạt động");
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}