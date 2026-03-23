using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using QuanLyHoatDongNgoaiKhoa.Services;
using QuanLyHoatDongNgoaiKhoa.Models;
using System.Security.Claims;

namespace QuanLyHoatDongNgoaiKhoa.Controllers
{
    [Authorize]
    public class ScheduleOptimizationController : Controller
    {
        private readonly IScheduleOptimizationService _scheduleService;
        private readonly AppDbContext _context;

        public ScheduleOptimizationController(IScheduleOptimizationService scheduleService, AppDbContext context)
        {
            _scheduleService = scheduleService;
            _context = context;
        }

        // GET: ScheduleOptimization/CheckConflict
        [HttpGet]
        public async Task<IActionResult> CheckConflict(int maHoatDong)
        {
            var hoatDong = await _context.HoatDongs
                .Include(h => h.DiaDiem)
                .Include(h => h.TrangThaiHoatDong)
                .FirstOrDefaultAsync(h => h.MaHoatDong == maHoatDong);

            if (hoatDong == null)
            {
                return NotFound();
            }

            var conflictResult = await _scheduleService.CheckScheduleConflictAsync(hoatDong, maHoatDong);
            
            return Json(new
            {
                hasConflict = conflictResult.HasConflict,
                conflictType = conflictResult.ConflictType.ToString(),
                conflictMessages = conflictResult.ConflictMessages,
                conflictingActivities = conflictResult.ConflictingActivities.Select(h => new
                {
                    h.MaHoatDong,
                    h.TieuDe,
                    h.ThoiGianBatDau,
                    h.ThoiGianKetThuc,
                    DiaDiem = h.DiaDiem?.TenDiaDiem,
                    TrangThai = h.TrangThaiHoatDong?.TenTrangThai
                })
            });
        }

        // GET: ScheduleOptimization/CheckUserConflict
        [HttpGet]
        public async Task<IActionResult> CheckUserConflict(int maHoatDong)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var hoatDong = await _context.HoatDongs
                .Include(h => h.DiaDiem)
                .FirstOrDefaultAsync(h => h.MaHoatDong == maHoatDong);

            if (hoatDong == null)
            {
                return NotFound();
            }

            var conflictResult = await _scheduleService.CheckUserScheduleConflictAsync(userId, hoatDong, maHoatDong);
            
            return Json(new
            {
                hasConflict = conflictResult.HasConflict,
                conflictType = conflictResult.ConflictType.ToString(),
                conflictMessages = conflictResult.ConflictMessages,
                conflictingActivities = conflictResult.ConflictingActivities.Select(h => new
                {
                    h.MaHoatDong,
                    h.TieuDe,
                    h.ThoiGianBatDau,
                    h.ThoiGianKetThuc,
                    DiaDiem = h.DiaDiem?.TenDiaDiem
                })
            });
        }

        // GET: ScheduleOptimization/SuggestTimeSlots
        [HttpGet]
        public async Task<IActionResult> SuggestTimeSlots(int maHoatDong)
        {
            var hoatDong = await _context.HoatDongs
                .Include(h => h.DiaDiem)
                .FirstOrDefaultAsync(h => h.MaHoatDong == maHoatDong);

            if (hoatDong == null)
            {
                return NotFound();
            }

            var suggestions = await _scheduleService.SuggestOptimalTimeSlotsAsync(hoatDong);
            
            return Json(suggestions.Select(s => new
            {
                startTime = s.StartTime.ToString("yyyy-MM-dd HH:mm"),
                endTime = s.EndTime.ToString("yyyy-MM-dd HH:mm"),
                diaDiemId = s.DiaDiemId,
                diaDiemName = s.DiaDiemName,
                score = s.Score
            }));
        }

        // GET: ScheduleOptimization/FindAvailableSlots
        [HttpGet]
        public async Task<IActionResult> FindAvailableSlots(int maDiaDiem, DateTime startDate, DateTime endDate, int durationMinutes)
        {
            var duration = TimeSpan.FromMinutes(durationMinutes);
            var slots = await _scheduleService.FindAvailableTimeSlotsAsync(maDiaDiem, startDate, endDate, duration);
            
            return Json(slots.Select(s => new
            {
                startTime = s.StartTime.ToString("yyyy-MM-dd HH:mm"),
                endTime = s.EndTime.ToString("yyyy-MM-dd HH:mm"),
                score = s.Score
            }));
        }

        // POST: ScheduleOptimization/CheckDuplicateRegistration
        [HttpPost]
        public async Task<IActionResult> CheckDuplicateRegistration(int maHoatDong)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var isDuplicate = await _scheduleService.CheckDuplicateRegistrationAsync(userId, maHoatDong);
            
            return Json(new { isDuplicate });
        }

        // GET: ScheduleOptimization/Calendar
        [HttpGet]
        public async Task<IActionResult> Calendar()
        {
            var hoatDongs = await _context.HoatDongs
                .Include(h => h.DiaDiem)
                .Include(h => h.TrangThaiHoatDong)
                .Include(h => h.DanhMucHoatDong)
                .Where(h => h.MaTrangThai != 3) // Không hiển thị hoạt động bị từ chối
                .OrderBy(h => h.ThoiGianBatDau)
                .ToListAsync();

            var calendarEvents = hoatDongs.Select(h => new
            {
                id = h.MaHoatDong,
                title = h.TieuDe,
                start = h.ThoiGianBatDau.ToString("yyyy-MM-ddTHH:mm:ss"),
                end = h.ThoiGianKetThuc.ToString("yyyy-MM-ddTHH:mm:ss"),
                location = h.DiaDiem?.TenDiaDiem,
                category = h.DanhMucHoatDong?.TenDanhMuc,
                status = h.TrangThaiHoatDong?.TenTrangThai,
                backgroundColor = GetEventColor(h.TrangThaiHoatDong?.MaTrangThai ?? 0),
                borderColor = GetEventColor(h.TrangThaiHoatDong?.MaTrangThai ?? 0)
            });

            return View(calendarEvents);
        }

        private string GetEventColor(int maTrangThai)
        {
            return maTrangThai switch
            {
                1 => "#ffc107", // Chờ xác nhận - Vàng
                2 => "#28a745", // Đã xác nhận - Xanh lá
                3 => "#dc3545", // Từ chối - Đỏ
                4 => "#6c757d", // Đã hủy - Xám
                _ => "#007bff"  // Mặc định - Xanh dương
            };
        }

        // GET: ScheduleOptimization/Statistics
        [HttpGet]
        public IActionResult Statistics()
        {
            return View();
        }

        // GET: ScheduleOptimization/GetStatistics
        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var totalActivities = await _context.HoatDongs.CountAsync();
                var totalRegistrations = await _context.DangKyThamGias.CountAsync();
                
                // Đếm xung đột (hoạt động có thời gian trùng lặp)
                var conflictCount = await _context.HoatDongs
                    .Where(h => h.MaTrangThai != 3)
                    .CountAsync(h => _context.HoatDongs
                        .Any(other => other.MaHoatDong != h.MaHoatDong &&
                                    other.MaDiaDiem == h.MaDiaDiem &&
                                    other.MaTrangThai != 3 &&
                                    other.ThoiGianBatDau < h.ThoiGianKetThuc &&
                                    other.ThoiGianKetThuc > h.ThoiGianBatDau));

                return Json(new
                {
                    totalActivities,
                    totalRegistrations,
                    conflictCount
                });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        // GET: ScheduleOptimization/GetRecentActivities
        [HttpGet]
        public async Task<IActionResult> GetRecentActivities()
        {
            try
            {
                var recentActivities = await _context.HoatDongs
                    .Include(h => h.DanhMucHoatDong)
                    .Include(h => h.DiaDiem)
                    .Include(h => h.TrangThaiHoatDong)
                    .Where(h => h.MaTrangThai != 3)
                    .OrderByDescending(h => h.NgayTao)
                    .Take(10)
                    .ToListAsync();

                var result = recentActivities.Select(h => new
                {
                    h.MaHoatDong,
                    h.TieuDe,
                    h.ThoiGianBatDau,
                    h.ThoiGianKetThuc,
                    danhMuc = h.DanhMucHoatDong != null ? h.DanhMucHoatDong.TenDanhMuc : "Chưa phân loại",
                    diaDiem = h.DiaDiem != null ? h.DiaDiem.TenDiaDiem : "Chưa cập nhật",
                    trangThai = h.TrangThaiHoatDong != null ? h.TrangThaiHoatDong.TenTrangThai : "Chưa cập nhật",
                    hasConflict = _context.HoatDongs
                        .Any(other => other.MaHoatDong != h.MaHoatDong &&
                                    other.MaDiaDiem == h.MaDiaDiem &&
                                    other.MaTrangThai != 3 &&
                                    other.ThoiGianBatDau < h.ThoiGianKetThuc &&
                                    other.ThoiGianKetThuc > h.ThoiGianBatDau)
                }).ToList();

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
    }
} 