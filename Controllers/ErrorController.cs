using Microsoft.AspNetCore.Mvc;
using QuanLyHoatDongNgoaiKhoa.ViewModels;

namespace QuanLyHoatDongNgoaiKhoa.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error")]
        public IActionResult Index(string? message = null, string? details = null, bool showDetails = false)
        {
            var errorViewModel = new ErrorViewModel
            {
                Message = message ?? "Đã xảy ra lỗi không mong muốn.",
                Details = details,
                ShowDetails = showDetails
            };

            return View(errorViewModel);
        }

        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            var errorViewModel = new ErrorViewModel();

            switch (statusCode)
            {
                case 404:
                    errorViewModel.Message = "Không tìm thấy trang bạn yêu cầu.";
                    break;
                case 403:
                    errorViewModel.Message = "Bạn không có quyền truy cập trang này.";
                    break;
                case 500:
                    errorViewModel.Message = "Đã xảy ra lỗi máy chủ. Vui lòng thử lại sau.";
                    break;
                default:
                    errorViewModel.Message = $"Đã xảy ra lỗi với mã {statusCode}.";
                    break;
            }

            return View("Index", errorViewModel);
        }
    }
} 