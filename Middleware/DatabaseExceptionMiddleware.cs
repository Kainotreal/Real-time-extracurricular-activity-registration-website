using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace QuanLyHoatDongNgoaiKhoa.Middleware
{
    public class DatabaseExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<DatabaseExceptionMiddleware> _logger;

        public DatabaseExceptionMiddleware(RequestDelegate next, ILogger<DatabaseExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (DbUpdateException dbEx) when (dbEx.InnerException is SqlException sqlEx)
            {
                _logger.LogError(dbEx, "Database error occurred");

                // Xử lý các lỗi database cụ thể
                string userMessage = GetUserFriendlyMessage(sqlEx);
                
                if (context.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    // AJAX request
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync($"{{\"error\":\"{userMessage}\"}}");
                }
                else
                {
                    // Regular request - redirect to error page
                    context.Response.Redirect($"/Error?message={Uri.EscapeDataString(userMessage)}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");
                throw;
            }
        }

        private string GetUserFriendlyMessage(SqlException sqlEx)
        {
            switch (sqlEx.Number)
            {
                case 547: // Foreign key constraint violation
                    if (sqlEx.Message.Contains("FK_DangKyThamGias_HoatDongs_MaHoatDong"))
                    {
                        return "Không thể xóa hoạt động vì đã có người đăng ký tham gia. Vui lòng xóa tất cả đăng ký trước khi xóa hoạt động.";
                    }
                    return "Không thể thực hiện thao tác này vì dữ liệu đang được sử dụng ở nơi khác.";
                
                case 2627: // Unique constraint violation
                    return "Dữ liệu này đã tồn tại trong hệ thống.";
                
                case 2601: // Unique index violation
                    return "Dữ liệu này đã tồn tại trong hệ thống.";
                
                case 4060: // Cannot open database
                    return "Không thể kết nối đến cơ sở dữ liệu. Vui lòng thử lại sau.";
                
                case 18456: // Login failed
                    return "Lỗi xác thực cơ sở dữ liệu.";
                
                default:
                    return "Đã xảy ra lỗi cơ sở dữ liệu. Vui lòng thử lại sau.";
            }
        }
    }
} 