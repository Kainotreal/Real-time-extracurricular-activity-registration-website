using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyHoatDongNgoaiKhoa.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
using QuanLyHoatDongNgoaiKhoa.Hubs;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using QuanLyHoatDongNgoaiKhoa.Services;

namespace QuanLyHoatDongNgoaiKhoa.Controllers
{
    [Authorize]
    public class DangKyThamGiaController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<HubThongBao> _hubContext;
        private readonly INotificationService _notificationService;
        private readonly IScheduleOptimizationService _scheduleService;

        public DangKyThamGiaController(AppDbContext context, IHubContext<HubThongBao> hubContext, INotificationService notificationService, IScheduleOptimizationService scheduleService)
        {
            _context = context;
            _hubContext = hubContext;
            _notificationService = notificationService;
            _scheduleService = scheduleService;
        }

        #region View Actions

        // GET: DangKyThamGia
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Index()
        {
            var dangKyThamGias = await _context.DangKyThamGias
                .Include(d => d.User)
                .Include(d => d.HoatDong)
                .ThenInclude(h => h.TrangThaiHoatDong)
                .Include(d => d.HoatDong)
                .ThenInclude(h => h.DanhMucHoatDong)
                .Include(d => d.HoatDong)
                .ThenInclude(h => h.LoaiHoatDong)
                .Include(d => d.HoatDong)
                .ThenInclude(h => h.DiaDiem)
                .Include(d => d.TrangThaiDangKy)
                .OrderByDescending(d => d.NgayDangKy)
                .ToListAsync();

            return View(dangKyThamGias);
        }

        // GET: DangKyThamGia/MyRegistrations
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> MyRegistrations()
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            var myRegistrations = await _context.DangKyThamGias
                .Include(d => d.User)
                .Include(d => d.HoatDong)
                .ThenInclude(h => h.TrangThaiHoatDong)
                .Include(d => d.HoatDong)
                .ThenInclude(h => h.DanhMucHoatDong)
                .Include(d => d.HoatDong)
                .ThenInclude(h => h.LoaiHoatDong)
                .Include(d => d.HoatDong)
                .ThenInclude(h => h.DiaDiem)
                .Include(d => d.TrangThaiDangKy)
                .Where(d => d.UserId == currentUserId)
                .OrderByDescending(d => d.NgayDangKy)
                .ToListAsync();

            return View(myRegistrations);
        }

        // GET: DangKyThamGia/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isStudent = User.IsInRole("Student");
            var isAdminOrTeacher = User.IsInRole("Admin") || User.IsInRole("Teacher");

            var query = _context.DangKyThamGias
                .Include(d => d.User)
                .Include(d => d.HoatDong)
                .ThenInclude(h => h.TrangThaiHoatDong)
                .Include(d => d.HoatDong)
                .ThenInclude(h => h.DanhMucHoatDong)
                .Include(d => d.HoatDong)
                .ThenInclude(h => h.LoaiHoatDong)
                .Include(d => d.HoatDong)
                .ThenInclude(h => h.DiaDiem)
                .Include(d => d.TrangThaiDangKy)
                .Where(d => d.MaDangKy == id);

            // Học sinh chỉ có thể xem đăng ký của mình
            if (isStudent)
            {
                query = query.Where(d => d.UserId == currentUserId);
            }

            var dangKyThamGia = await query.FirstOrDefaultAsync();

            if (dangKyThamGia == null)
            {
                return NotFound();
            }

            return View(dangKyThamGia);
        }

        // GET: DangKyThamGia/Create
        [Authorize(Roles = "Student")]
public async Task<IActionResult> Create(int? maHoatDong)
{
    if (maHoatDong == null)
    {
        return NotFound();
    }

    var hoatDong = await _context.HoatDongs
        .Include(h => h.TrangThaiHoatDong)
        .Include(h => h.DanhMucHoatDong)
        .Include(h => h.LoaiHoatDong)
        .Include(h => h.DiaDiem)
        .FirstOrDefaultAsync(h => h.MaHoatDong == maHoatDong);

    if (hoatDong == null)
    {
        return NotFound();
    }

    // Kiểm tra và in ra các thuộc tính null trong hoatDong
    var nullProperties = new List<string>();
    if (hoatDong.TrangThaiHoatDong == null) nullProperties.Add(nameof(hoatDong.TrangThaiHoatDong));
    if (hoatDong.DanhMucHoatDong == null) nullProperties.Add(nameof(hoatDong.DanhMucHoatDong));
    if (hoatDong.LoaiHoatDong == null) nullProperties.Add(nameof(hoatDong.LoaiHoatDong));
    if (hoatDong.DiaDiem == null) nullProperties.Add(nameof(hoatDong.DiaDiem));

    if (nullProperties.Any())
    {
        TempData["WarningMessage"] = "Một số dữ liệu hoạt động đang bị thiếu: " + string.Join(", ", nullProperties);
    }

    // Kiểm tra xem học sinh đã đăng ký chưa
    var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    var existingRegistration = await _context.DangKyThamGias
        .FirstOrDefaultAsync(d => d.MaHoatDong == maHoatDong && d.UserId == currentUserId);

    if (existingRegistration != null)
    {
        TempData["ErrorMessage"] = "Bạn đã đăng ký tham gia hoạt động này rồi!";
        return RedirectToAction("Details", "HoatDong", new { id = maHoatDong });
    }

    // Kiểm tra điều kiện đăng ký
    if (!await ValidateRegistrationConditions(hoatDong))
    {
        return RedirectToAction("Details", "HoatDong", new { id = maHoatDong });
    }

    ViewBag.HoatDong = hoatDong;
    ViewBag.TrangThaiDangKys = await _context.TrangThaiDangKys.ToListAsync();

    return View();
}


        // POST: DangKyThamGia/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Create(int maHoatDong, string? ghiChu)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                if (string.IsNullOrEmpty(currentUserId))
                {
                    TempData["ErrorMessage"] = "Không xác định được người dùng!";
                    return RedirectToAction("Details", "HoatDong", new { id = maHoatDong });
                }

                // Kiểm tra hoạt động tồn tại
                var hoatDong = await _context.HoatDongs.FindAsync(maHoatDong);
                if (hoatDong == null)
                {
                    TempData["ErrorMessage"] = "Hoạt động không tồn tại!";
                    return RedirectToAction("Index", "HoatDong");
                }

                // Kiểm tra trùng lặp đăng ký
                var isDuplicate = await _scheduleService.CheckDuplicateRegistrationAsync(currentUserId, maHoatDong);
                if (isDuplicate)
                {
                    TempData["ErrorMessage"] = "Bạn đã đăng ký tham gia hoạt động này rồi!";
                    return RedirectToAction("Details", "HoatDong", new { id = maHoatDong });
                }

                // Kiểm tra xung đột lịch cá nhân
                if (hoatDong != null)
                {
                    var userConflictResult = await _scheduleService.CheckUserScheduleConflictAsync(currentUserId, hoatDong);
                    if (userConflictResult.HasConflict)
                    {
                        TempData["ErrorMessage"] = string.Join("; ", userConflictResult.ConflictMessages);
                        return RedirectToAction("Details", "HoatDong", new { id = maHoatDong });
                    }
                }

                // Kiểm tra điều kiện đăng ký
                var validationResult = await ValidateRegistrationConditionsAsync(hoatDong);
                if (!validationResult.IsValid)
                {
                    TempData["ErrorMessage"] = validationResult.ErrorMessage;
                    return RedirectToAction("Details", "HoatDong", new { id = maHoatDong });
                }

                // Tạo đăng ký mới với trạng thái "Chờ xác nhận" (ID = 1)
                var newRegistration = new DangKyThamGia();
                newRegistration.MaHoatDong = maHoatDong;
                newRegistration.UserId = currentUserId;
                newRegistration.MaTrangThaiDangKy = 1; // 1 = "Chờ xác nhận"
                newRegistration.NgayDangKy = DateTime.Now;
                newRegistration.GhiChu = ghiChu ?? string.Empty;

                // Log thông tin trước khi lưu
                Console.WriteLine($"Creating registration: MaHoatDong={maHoatDong}, UserId={currentUserId}, MaTrangThaiDangKy=1");
                Console.WriteLine($"Registration object created: {newRegistration.MaHoatDong}, {newRegistration.UserId}, {newRegistration.MaTrangThaiDangKy}, {newRegistration.NgayDangKy}");

                // Thử cách khác - sử dụng SQL trực tiếp
                var sql = @"INSERT INTO DangKyThamGias (UserId, MaHoatDong, MaTrangThaiDangKy, NgayDangKy, GhiChu) 
                           VALUES (@UserId, @MaHoatDong, @MaTrangThaiDangKy, @NgayDangKy, @GhiChu);
                           SELECT CAST(SCOPE_IDENTITY() as int)";
                
                var parameters = new[]
                {
                    new SqlParameter("@UserId", currentUserId),
                    new SqlParameter("@MaHoatDong", maHoatDong),
                    new SqlParameter("@MaTrangThaiDangKy", 1),
                    new SqlParameter("@NgayDangKy", DateTime.Now),
                    new SqlParameter("@GhiChu", ghiChu ?? string.Empty)
                };
                
                try
                {
                    var newId = await _context.Database.ExecuteSqlRawAsync(sql, parameters);
                    Console.WriteLine($"Registration created with ID: {newId}");
                }
                catch (Exception sqlEx)
                {
                    Console.WriteLine($"SQL error: {sqlEx.Message}");
                    Console.WriteLine($"Inner exception: {sqlEx.InnerException?.Message}");
                    throw;
                }
                
                // Log sau khi SaveChanges thành công
                Console.WriteLine($"Registration created successfully with ID: {newRegistration.MaDangKy}");

                // Gửi thông báo realtime
                await SendRealtimeNotifications(newRegistration, hoatDong, "create");
                
                // Gửi thông báo qua NotificationService
                var studentName = User.Identity?.Name ?? "Sinh viên";
                await _notificationService.SendRegistrationNotification(newRegistration, hoatDong, studentName);

                TempData["SuccessMessage"] = "Đăng ký tham gia thành công! Vui lòng chờ xác nhận.";
                return RedirectToAction("Details", "HoatDong", new { id = maHoatDong });
            }
            catch (Exception ex)
            {
                // Log chi tiết lỗi
                Console.WriteLine($"Error creating registration: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                
                TempData["ErrorMessage"] = $"Lỗi khi đăng ký: {ex.Message}";
                return RedirectToAction("Details", "HoatDong", new { id = maHoatDong });
            }
        }

        // GET: DangKyThamGia/XacNhanList
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> XacNhanList()
        {
            var dangKyThamGias = await _context.DangKyThamGias
                .Include(d => d.User)
                .Include(d => d.HoatDong)
                .ThenInclude(h => h.TrangThaiHoatDong)
                .Include(d => d.HoatDong)
                .ThenInclude(h => h.DanhMucHoatDong)
                .Include(d => d.HoatDong)
                .ThenInclude(h => h.LoaiHoatDong)
                .Include(d => d.HoatDong)
                .ThenInclude(h => h.DiaDiem)
                .Include(d => d.TrangThaiDangKy)
                .Where(d => d.MaTrangThaiDangKy == 1) // 1 = "Chờ xác nhận"
                .OrderByDescending(d => d.NgayDangKy)
                .ToListAsync();

            // Nếu là AJAX request, trả về partial view
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_XacNhanListPartial", dangKyThamGias);
            }

            return View(dangKyThamGias);
        }

        // GET: DangKyThamGia/XacNhan/5
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> XacNhan(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dangKyThamGia = await _context.DangKyThamGias
                .Include(d => d.User)
                .Include(d => d.HoatDong)
                .ThenInclude(h => h.TrangThaiHoatDong)
                .Include(d => d.HoatDong)
                .ThenInclude(h => h.DanhMucHoatDong)
                .Include(d => d.HoatDong)
                .ThenInclude(h => h.LoaiHoatDong)
                .Include(d => d.HoatDong)
                .ThenInclude(h => h.DiaDiem)
                .Include(d => d.TrangThaiDangKy)
                .FirstOrDefaultAsync(d => d.MaDangKy == id);

            if (dangKyThamGia == null)
            {
                return NotFound();
            }

            return View(dangKyThamGia);
        }

        // POST: DangKyThamGia/XacNhan/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> XacNhan(int id, bool dongY)
        {
            try
            {
                var dangKyThamGia = await _context.DangKyThamGias
                    .Include(d => d.HoatDong)
                    .FirstOrDefaultAsync(d => d.MaDangKy == id);

                if (dangKyThamGia == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy đăng ký!";
                    return RedirectToAction("XacNhanList");
                }

                var oldStatus = dangKyThamGia.MaTrangThaiDangKy;

                if (dongY)
                {
                    dangKyThamGia.MaTrangThaiDangKy = 2; // 2 = "Đã xác nhận"
                }
                else
                {
                    dangKyThamGia.MaTrangThaiDangKy = 3; // 3 = "Từ chối"
                }

                await _context.SaveChangesAsync();

                // Gửi thông báo realtime
                await SendRealtimeNotifications(dangKyThamGia, dangKyThamGia.HoatDong, "confirm");
                
                // Gửi thông báo qua NotificationService
                var status = dongY ? "xác nhận" : "từ chối";
                await _notificationService.SendRegistrationConfirmationNotification(dangKyThamGia, dangKyThamGia.HoatDong, status);

                var message = dongY ? "Đã xác nhận đăng ký thành công!" : "Đã từ chối đăng ký!";
                
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true, message = message, id = dangKyThamGia.MaDangKy });
                }

                TempData["SuccessMessage"] = message;
                return RedirectToAction("XacNhanList");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi xử lý đăng ký: {ex.Message}";
                return RedirectToAction("XacNhanList");
            }
        }

        // GET: DangKyThamGia/Delete/5
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            var dangKyThamGia = await _context.DangKyThamGias
                .Include(d => d.User)
                .Include(d => d.HoatDong)
                .ThenInclude(h => h.TrangThaiHoatDong)
                .Include(d => d.HoatDong)
                .ThenInclude(h => h.DanhMucHoatDong)
                .Include(d => d.HoatDong)
                .ThenInclude(h => h.LoaiHoatDong)
                .Include(d => d.HoatDong)
                .ThenInclude(h => h.DiaDiem)
                .Include(d => d.TrangThaiDangKy)
                .FirstOrDefaultAsync(d => d.MaDangKy == id && d.UserId == currentUserId);

            if (dangKyThamGia == null)
            {
                return NotFound();
            }

            return View(dangKyThamGia);
        }

        // POST: DangKyThamGia/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                var dangKyThamGia = await _context.DangKyThamGias
                    .Include(d => d.HoatDong)
                    .FirstOrDefaultAsync(d => d.MaDangKy == id && d.UserId == currentUserId);

                if (dangKyThamGia == null)
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, message = "Không tìm thấy đăng ký hoặc bạn không có quyền hủy!" });
                    }
                    TempData["ErrorMessage"] = "Không tìm thấy đăng ký hoặc bạn không có quyền hủy!";
                    return RedirectToAction("MyRegistrations");
                }

                var hoatDong = dangKyThamGia.HoatDong;
                _context.DangKyThamGias.Remove(dangKyThamGia);
                await _context.SaveChangesAsync();

                // Gửi thông báo realtime
                await SendRealtimeNotifications(dangKyThamGia, hoatDong, "delete");

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { 
                        success = true, 
                        message = "Hủy đăng ký thành công!", 
                        id = id,
                        activityName = hoatDong?.TieuDe,
                        redirectUrl = Url.Action("Details", "HoatDong", new { id = hoatDong?.MaHoatDong })
                    });
                }
                
                TempData["SuccessMessage"] = "Hủy đăng ký thành công!";
                return RedirectToAction("Details", "HoatDong", new { id = hoatDong?.MaHoatDong });
            }
            catch (Exception ex)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = $"Lỗi khi hủy đăng ký: {ex.Message}" });
                }
                TempData["ErrorMessage"] = $"Lỗi khi hủy đăng ký: {ex.Message}";
                return RedirectToAction("MyRegistrations");
            }
        }

        // GET: DangKyThamGia/Edit/5
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            var dangKyThamGia = await _context.DangKyThamGias
                .Include(d => d.User)
                .Include(d => d.HoatDong)
                .ThenInclude(h => h.TrangThaiHoatDong)
                .Include(d => d.HoatDong)
                .ThenInclude(h => h.DanhMucHoatDong)
                .Include(d => d.HoatDong)
                .ThenInclude(h => h.LoaiHoatDong)
                .Include(d => d.HoatDong)
                .ThenInclude(h => h.DiaDiem)
                .Include(d => d.TrangThaiDangKy)
                .FirstOrDefaultAsync(d => d.MaDangKy == id && d.UserId == currentUserId);

            if (dangKyThamGia == null)
            {
                return NotFound();
            }

            ViewBag.TrangThaiDangKys = await _context.TrangThaiDangKys.ToListAsync();
            return View(dangKyThamGia);
        }

        // POST: DangKyThamGia/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Edit(int id, [Bind("MaDangKy,UserId,MaHoatDong,MaTrangThaiDangKy,NgayDangKy,GhiChu")] DangKyThamGia dangKyThamGia)
        {
            if (id != dangKyThamGia.MaDangKy)
            {
                return NotFound();
            }

            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            // Kiểm tra quyền sở hữu
            var existingRegistration = await _context.DangKyThamGias
                .FirstOrDefaultAsync(d => d.MaDangKy == id && d.UserId == currentUserId);

            if (existingRegistration == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Chỉ cho phép cập nhật ghi chú và trạng thái đăng ký
                    existingRegistration.GhiChu = dangKyThamGia.GhiChu;
                    existingRegistration.MaTrangThaiDangKy = dangKyThamGia.MaTrangThaiDangKy;

                    _context.Update(existingRegistration);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Cập nhật đăng ký thành công!";
                    return RedirectToAction("MyRegistrations");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DangKyThamGiaExists(dangKyThamGia.MaDangKy))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewBag.TrangThaiDangKys = await _context.TrangThaiDangKys.ToListAsync();
            return View(dangKyThamGia);
        }

        // POST: DangKyThamGia/CancelMyRegistration/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> CancelMyRegistration(int id)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                var dangKyThamGia = await _context.DangKyThamGias
                    .Include(d => d.HoatDong)
                    .FirstOrDefaultAsync(d => d.MaDangKy == id && d.UserId == currentUserId);

                if (dangKyThamGia == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy đăng ký hoặc bạn không có quyền hủy!";
                    return RedirectToAction("MyRegistrations");
                }

                var hoatDong = dangKyThamGia.HoatDong;
                _context.DangKyThamGias.Remove(dangKyThamGia);
                await _context.SaveChangesAsync();

                // Gửi thông báo realtime
                await SendRealtimeNotifications(dangKyThamGia, hoatDong, "delete");

                TempData["SuccessMessage"] = "Hủy đăng ký thành công!";
                return RedirectToAction("MyRegistrations");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi hủy đăng ký: {ex.Message}";
                return RedirectToAction("MyRegistrations");
            }
        }

        #endregion

        #region API Actions

        // API để lấy thông tin user cho JavaScript
        [HttpGet]
        [Route("api/user/info")]
        public IActionResult GetUserInfo()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;

            return Json(new
            {
                userId = userId,
                role = userRole,
                userName = userName
            });
        }

        // API để lấy số lượng đăng ký realtime
        [HttpGet]
        [Route("api/dangky/soluong/{maHoatDong}")]
        public async Task<IActionResult> GetSoLuongDangKy(int maHoatDong)
        {
            try
            {
                var soLuongDangKy = await _context.DangKyThamGias
                    .Where(d => d.MaHoatDong == maHoatDong && d.MaTrangThaiDangKy == 2) // 2 = "Đã xác nhận"
                    .CountAsync();

                var hoatDong = await _context.HoatDongs.FindAsync(maHoatDong);
                var soLuongToiDa = hoatDong?.SoLuongToiDa ?? 0;

                return Json(new { 
                    success = true,
                    soLuongHienTai = soLuongDangKy, 
                    soLuongToiDa = soLuongToiDa,
                    conCho = soLuongToiDa - soLuongDangKy,
                    tiLe = soLuongToiDa > 0 ? (double)soLuongDangKy / soLuongToiDa * 100 : 0
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // API để kiểm tra trạng thái đăng ký của user
        [HttpGet]
        [Route("api/dangky/check/{maHoatDong}")]
        public async Task<IActionResult> CheckUserRegistration(int maHoatDong)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                var registration = await _context.DangKyThamGias
                    .Where(d => d.MaHoatDong == maHoatDong && d.UserId == currentUserId)
                    .Select(d => new { d.MaDangKy, d.MaTrangThaiDangKy, d.NgayDangKy, d.GhiChu })
                    .FirstOrDefaultAsync();

                return Json(new { 
                    success = true,
                    hasRegistered = registration != null,
                    registration = registration
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // API để đăng ký hoạt động qua AJAX
        [HttpPost]
        [Route("api/dangky/register")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> RegisterActivity([FromBody] DangKyThamGiaRequest request)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return Json(new { success = false, message = "Không xác định được người dùng!" });
                }

                // Kiểm tra hoạt động tồn tại
                var hoatDong = await _context.HoatDongs.FindAsync(request.MaHoatDong);
                if (hoatDong == null)
                {
                    return Json(new { success = false, message = "Hoạt động không tồn tại!" });
                }

                // Kiểm tra đã đăng ký chưa
                var existingRegistration = await _context.DangKyThamGias
                    .FirstOrDefaultAsync(d => d.MaHoatDong == request.MaHoatDong && d.UserId == currentUserId);

                if (existingRegistration != null)
                {
                    return Json(new { success = false, message = "Bạn đã đăng ký tham gia hoạt động này rồi!" });
                }

                // Kiểm tra điều kiện đăng ký
                var validationResult = await ValidateRegistrationConditionsAsync(hoatDong);
                if (!validationResult.IsValid)
                {
                    return Json(new { success = false, message = validationResult.ErrorMessage });
                }

                // Tạo đăng ký mới với trạng thái "Chờ xác nhận" (ID = 1)
                var newRegistration = new DangKyThamGia();
                newRegistration.MaHoatDong = request.MaHoatDong;
                newRegistration.UserId = currentUserId;
                newRegistration.MaTrangThaiDangKy = 1; // 1 = "Chờ xác nhận"
                newRegistration.NgayDangKy = DateTime.Now;
                newRegistration.GhiChu = request.GhiChu ?? string.Empty;

                // Sử dụng Entity Framework để insert và lấy ID
                _context.DangKyThamGias.Add(newRegistration);
                await _context.SaveChangesAsync();

                // Gửi thông báo realtime
                await SendRealtimeNotifications(newRegistration, hoatDong, "create");

                return Json(new { 
                    success = true, 
                    message = "Đăng ký tham gia thành công! Vui lòng chờ xác nhận.",
                    registrationId = newRegistration.MaDangKy
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi khi đăng ký: {ex.Message}" });
            }
        }

        // API để hủy đăng ký qua AJAX
        [HttpPost]
        [Route("api/dangky/cancel/{id}")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> CancelRegistration(int id)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                var dangKyThamGia = await _context.DangKyThamGias
                    .Include(d => d.HoatDong)
                    .FirstOrDefaultAsync(d => d.MaDangKy == id && d.UserId == currentUserId);

                if (dangKyThamGia == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đăng ký hoặc bạn không có quyền hủy!" });
                }

                var hoatDong = dangKyThamGia.HoatDong;
                _context.DangKyThamGias.Remove(dangKyThamGia);
                await _context.SaveChangesAsync();

                // Gửi thông báo realtime
                await SendRealtimeNotifications(dangKyThamGia, hoatDong, "delete");

                return Json(new { 
                    success = true, 
                    message = "Hủy đăng ký thành công!" 
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi khi hủy đăng ký: {ex.Message}" });
            }
        }

        // API để xác nhận/từ chối đăng ký qua AJAX
        [HttpPost]
        [Route("api/dangky/confirm/{id}")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> ConfirmRegistration(int id, [FromBody] ConfirmRequest request)
        {
            try
            {
                var dangKyThamGia = await _context.DangKyThamGias
                    .Include(d => d.HoatDong)
                    .FirstOrDefaultAsync(d => d.MaDangKy == id);

                if (dangKyThamGia == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đăng ký!" });
                }

                var oldStatus = dangKyThamGia.MaTrangThaiDangKy;

                if (request.DongY)
                {
                    dangKyThamGia.MaTrangThaiDangKy = 2; // 2 = "Đã xác nhận"
                }
                else
                {
                    dangKyThamGia.MaTrangThaiDangKy = 3; // 3 = "Từ chối"
                }

                await _context.SaveChangesAsync();

                // Gửi thông báo realtime
                await SendRealtimeNotifications(dangKyThamGia, dangKyThamGia.HoatDong, "confirm");

                var message = request.DongY ? "Đã xác nhận đăng ký thành công!" : "Đã từ chối đăng ký!";
                return Json(new { success = true, message = message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi khi xử lý đăng ký: {ex.Message}" });
            }
        }

        // API để lấy danh sách đăng ký chờ xác nhận
        [HttpGet]
        [Route("api/dangky/pending")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> GetPendingRegistrations()
        {
            try
            {
                                 var pendingRegistrations = await _context.DangKyThamGias
                     .Include(d => d.User)
                     .Include(d => d.HoatDong)
                     .Include(d => d.TrangThaiDangKy)
                     .Where(d => d.MaTrangThaiDangKy == 1) // 1 = "Chờ xác nhận"
                     .OrderByDescending(d => d.NgayDangKy)
                     .Select(d => new
                     {
                         d.MaDangKy,
                         d.MaHoatDong,
                         d.UserId,
                         d.NgayDangKy,
                         d.GhiChu,
                         User = new
                         {
                             d.User.Id,
                             d.User.UserName,
                             d.User.Email
                         },
                         HoatDong = new
                         {
                             d.HoatDong.MaHoatDong,
                             d.HoatDong.TieuDe,
                             d.HoatDong.ThoiGianBatDau,
                             d.HoatDong.ThoiGianKetThuc,
                             d.HoatDong.SoLuongToiDa
                         },
                         TrangThai = d.TrangThaiDangKy.TenTrangThaiDangKy
                     })
                     .ToListAsync();

                return Json(new { success = true, data = pendingRegistrations });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // API để lấy thống kê đăng ký
        [HttpGet]
        [Route("api/dangky/stats/{maHoatDong}")]
        public async Task<IActionResult> GetRegistrationStats(int maHoatDong)
        {
            try
            {
                var stats = await _context.DangKyThamGias
                    .Where(d => d.MaHoatDong == maHoatDong)
                    .GroupBy(d => d.MaTrangThaiDangKy)
                    .Select(g => new
                    {
                        TrangThai = g.Key,
                        SoLuong = g.Count()
                    })
                    .ToListAsync();

                var hoatDong = await _context.HoatDongs.FindAsync(maHoatDong);
                var soLuongToiDa = hoatDong?.SoLuongToiDa ?? 0;

                return Json(new { 
                    success = true, 
                    stats = stats,
                    soLuongToiDa = soLuongToiDa
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // API để lấy số lượng đăng ký chờ xác nhận
        [HttpGet]
        [Route("api/dangky/pending-count")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> GetPendingCount()
        {
            try
            {
                var count = await _context.DangKyThamGias
                    .Where(d => d.MaTrangThaiDangKy == 1) // 1 = "Chờ xác nhận"
                    .CountAsync();

                return Json(new { success = true, count = count });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Helper Methods

        private async Task<bool> ValidateRegistrationConditions(HoatDong hoatDong)
        {
            if (hoatDong.ThoiGianBatDau <= DateTime.Now)
            {
                TempData["ErrorMessage"] = "Hoạt động đã bắt đầu, không thể đăng ký!";
                return false;
            }

            if (hoatDong.MaTrangThai != 2) // 2 = "Đã duyệt"
            {
                TempData["ErrorMessage"] = "Hoạt động chưa được duyệt, không thể đăng ký!";
                return false;
            }

            // Kiểm tra số lượng đăng ký
            var soLuongDangKy = await _context.DangKyThamGias
                .Where(d => d.MaHoatDong == hoatDong.MaHoatDong && d.MaTrangThaiDangKy == 2) // 2 = "Đã xác nhận"
                .CountAsync();

            if (soLuongDangKy >= hoatDong.SoLuongToiDa)
            {
                TempData["ErrorMessage"] = "Hoạt động đã đủ số lượng đăng ký!";
                return false;
            }

            return true;
        }

        private async Task<ValidationResult> ValidateRegistrationConditionsAsync(HoatDong hoatDong)
        {
            if (hoatDong.ThoiGianBatDau <= DateTime.Now)
            {
                return new ValidationResult { IsValid = false, ErrorMessage = "Hoạt động đã bắt đầu, không thể đăng ký!" };
            }

            if (hoatDong.MaTrangThai != 2) // 2 = "Đã duyệt"
            {
                return new ValidationResult { IsValid = false, ErrorMessage = "Hoạt động chưa được duyệt, không thể đăng ký!" };
            }

            // Kiểm tra số lượng đăng ký
            var soLuongDangKy = await _context.DangKyThamGias
                .Where(d => d.MaHoatDong == hoatDong.MaHoatDong && d.MaTrangThaiDangKy == 2) // 2 = "Đã xác nhận"
                .CountAsync();

            if (soLuongDangKy >= hoatDong.SoLuongToiDa)
            {
                return new ValidationResult { IsValid = false, ErrorMessage = "Hoạt động đã đủ số lượng đăng ký!" };
            }

            return new ValidationResult { IsValid = true };
        }

        private async Task SendRealtimeNotifications(DangKyThamGia dangKy, HoatDong hoatDong, string action)
        {
            try
            {
                Console.WriteLine($"Sending realtime notification: action={action}, hoatDong={hoatDong?.TieuDe}");
                
                if (action == "create")
                {
                    // Gửi thông báo cho admin
                    var adminNotification = new
                    {
                        id = Guid.NewGuid().ToString(),
                        title = "Đăng ký mới",
                        message = $"Có đăng ký mới cho hoạt động: {hoatDong.TieuDe}",
                        type = "success",
                        timestamp = DateTime.Now.ToString("HH:mm:ss")
                    };
                    Console.WriteLine($"Sending admin notification: {adminNotification.message}");
                    await _hubContext.Clients.Group("Admin").SendAsync("ReceiveNotification", adminNotification);
                    
                    // Gửi thông báo cho giáo viên tạo hoạt động
                    if (!string.IsNullOrEmpty(hoatDong.NguoiTaoId))
                    {
                        var teacherNotification = new
                        {
                            id = Guid.NewGuid().ToString(),
                            title = "Đăng ký mới",
                            message = $"Có học sinh đăng ký tham gia hoạt động của bạn: {hoatDong.TieuDe}",
                            type = "info",
                            timestamp = DateTime.Now.ToString("HH:mm:ss")
                        };
                        Console.WriteLine($"Sending teacher notification: {teacherNotification.message}");
                        await _hubContext.Clients.Group($"User_{hoatDong.NguoiTaoId}").SendAsync("ReceiveNotification", teacherNotification);
                    }
                }
                else if (action == "confirm")
                {
                    // Gửi thông báo cho sinh viên khi đăng ký được xác nhận/từ chối
                    var status = dangKy.MaTrangThaiDangKy == 2 ? "xác nhận" : "từ chối";
                    var message = $"Đăng ký tham gia hoạt động {hoatDong.TieuDe} của bạn đã được {status}";
                    var type = dangKy.MaTrangThaiDangKy == 2 ? "success" : "warning";
                    
                    var studentNotification = new
                    {
                        id = Guid.NewGuid().ToString(),
                        title = "Xác nhận đăng ký",
                        message = message,
                        type = type,
                        timestamp = DateTime.Now.ToString("HH:mm:ss")
                    };
                    Console.WriteLine($"Sending student notification: {studentNotification.message}");
                    await _hubContext.Clients.Group($"User_{dangKy.UserId}").SendAsync("ReceiveNotification", studentNotification);
                    
                    // Gửi thông báo cho admin về việc xác nhận
                    var adminNotification = new
                    {
                        id = Guid.NewGuid().ToString(),
                        title = "Xác nhận đăng ký",
                        message = $"Đã {status} đăng ký tham gia hoạt động: {hoatDong.TieuDe}",
                        type = "info",
                        timestamp = DateTime.Now.ToString("HH:mm:ss")
                    };
                    Console.WriteLine($"Sending admin confirmation notification: {adminNotification.message}");
                    await _hubContext.Clients.Group("Admin").SendAsync("ReceiveNotification", adminNotification);
                }
                else if (action == "delete")
                {
                    // Gửi thông báo khi sinh viên hủy đăng ký
                    var adminNotification = new
                    {
                        id = Guid.NewGuid().ToString(),
                        title = "Hủy đăng ký",
                        message = $"Sinh viên đã hủy đăng ký tham gia hoạt động: {hoatDong.TieuDe}",
                        type = "warning",
                        timestamp = DateTime.Now.ToString("HH:mm:ss")
                    };
                    Console.WriteLine($"Sending admin delete notification: {adminNotification.message}");
                    await _hubContext.Clients.Group("Admin").SendAsync("ReceiveNotification", adminNotification);
                    
                    // Gửi thông báo cho giáo viên tạo hoạt động
                    if (!string.IsNullOrEmpty(hoatDong.NguoiTaoId))
                    {
                        var teacherNotification = new
                        {
                            id = Guid.NewGuid().ToString(),
                            title = "Hủy đăng ký",
                            message = $"Có sinh viên hủy đăng ký tham gia hoạt động: {hoatDong.TieuDe}",
                            type = "warning",
                            timestamp = DateTime.Now.ToString("HH:mm:ss")
                        };
                        Console.WriteLine($"Sending teacher delete notification: {teacherNotification.message}");
                        await _hubContext.Clients.Group($"User_{hoatDong.NguoiTaoId}").SendAsync("ReceiveNotification", teacherNotification);
                    }
                }
                
                Console.WriteLine($"Realtime notification sent successfully for action: {action}");
            }
            catch (Exception ex)
            {
                // Không throw exception để không ảnh hưởng đến quá trình chính
                Console.WriteLine($"Error sending realtime notification: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }





        private bool DangKyThamGiaExists(int id)
        {
            return _context.DangKyThamGias.Any(e => e.MaDangKy == id);
        }

        

        #endregion

        #region Model Classes

        public class DangKyThamGiaRequest
        {
            public int MaHoatDong { get; set; }
            public string? GhiChu { get; set; }
        }

        public class ConfirmRequest
        {
            public bool DongY { get; set; }
        }

        public class ValidationResult
        {
            public bool IsValid { get; set; }
            public string ErrorMessage { get; set; } = string.Empty;
        }

        #endregion
    }
}
