using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyHoatDongNgoaiKhoa.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace QuanLyHoatDongNgoaiKhoa.Controllers
{
    [Authorize]
    public class ThongBaoController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ThongBaoController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: ThongBao/MyNotifications
        public async Task<IActionResult> MyNotifications()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Kiểm tra vai trò của user hiện tại
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var userRoles = await _userManager.GetRolesAsync(user);

            IQueryable<ThongBao> thongBaosQuery;

            if (userRoles.Contains("Admin"))
            {
                // Admin: Xem tất cả thông báo
                thongBaosQuery = _context.ThongBaos
                    .Include(t => t.HoatDong)
                    .ThenInclude(h => h.NguoiTao)
                    .OrderByDescending(t => t.NgayTao);
            }
            else
            {
                // Teacher/Student: Chỉ xem thông báo từ hoạt động của chính mình
                thongBaosQuery = _context.ThongBaos
                    .Include(t => t.HoatDong)
                    .ThenInclude(h => h.NguoiTao)
                    .Where(t => 
                        // Thông báo từ hoạt động mà user tạo
                        (t.HoatDong != null && t.HoatDong.NguoiTaoId == userId) ||
                        // Thông báo gửi trực tiếp cho user
                        t.NguoiGuiId == userId
                    )
                    .OrderByDescending(t => t.NgayTao);
            }

            var thongBaos = await thongBaosQuery
                .Take(50) // Lấy 50 thông báo gần nhất
                .ToListAsync();

            return View(thongBaos);
        }

        // POST: ThongBao/MarkAsRead
        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var thongBao = await _context.ThongBaos.FindAsync(id);
            if (thongBao != null)
            {
                thongBao.DaGui = true;
                thongBao.ThoiGianGui = DateTime.Now;
                await _context.SaveChangesAsync();
            }

            return Json(new { success = true });
        }

        // GET: ThongBao/GetUnreadCount
        [HttpGet]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { count = 0 });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Json(new { count = 0 });
            }
            var userRoles = await _userManager.GetRolesAsync(user);

            IQueryable<ThongBao> unreadQuery;

            if (userRoles.Contains("Admin"))
            {
                unreadQuery = _context.ThongBaos.Where(t => !t.DaGui);
            }
            else
            {
                unreadQuery = _context.ThongBaos
                    .Where(t => !t.DaGui && (
                        (t.HoatDong != null && t.HoatDong.NguoiTaoId == userId) ||
                        t.NguoiGuiId == userId
                    ));
            }

            var count = await unreadQuery.CountAsync();
            return Json(new { count = count });
        }
    }
}
