using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyHoatDongNgoaiKhoa.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyHoatDongNgoaiKhoa.Services;
using QuanLyHoatDongNgoaiKhoa.ViewModels;
using System.Collections.Generic; // Added for List<object>

namespace QuanLyHoatDongNgoaiKhoa.Controllers
{
    public class HoatDongController : Controller
    {
        private readonly AppDbContext _context;
        private readonly INotificationService _notificationService;
        private readonly IScheduleOptimizationService _scheduleService;
        private readonly IRecommenderService _recommenderService;

        public HoatDongController(AppDbContext context, INotificationService notificationService, IScheduleOptimizationService scheduleService, IRecommenderService recommenderService)
        {
            _context = context;
            _notificationService = notificationService;
            _scheduleService = scheduleService;
            _recommenderService = recommenderService;
        }

        // GET: HoatDong
        public async Task<IActionResult> Index()
        {
            var hoatDongs = await _context.HoatDongs
                .Include(h => h.TrangThaiHoatDong)
                .Include(h => h.DanhMucHoatDong)
                .Include(h => h.LoaiHoatDong)
                .Include(h => h.DiaDiem)
                .Include(h => h.NguoiTao)
                .Include(h => h.DanhSachDangKy)
                .ThenInclude(d => d.TrangThaiDangKy)
                .OrderByDescending(h => h.NgayTao)
                .ToListAsync();

            return View(hoatDongs);
        }

        // GET: HoatDong/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoatDong = await _context.HoatDongs
                .Include(h => h.TrangThaiHoatDong)
                .Include(h => h.DanhMucHoatDong)
                .Include(h => h.LoaiHoatDong)
                .Include(h => h.DiaDiem)
                .Include(h => h.NguoiTao)
                .Include(h => h.DanhSachDangKy)
                .ThenInclude(d => d.TrangThaiDangKy)
                .Include(h => h.DanhSachDangKy)
                .ThenInclude(d => d.User)
                .Include(h => h.DanhSachFile)
                .Include(h => h.DanhSachThongBao)
                .FirstOrDefaultAsync(m => m.MaHoatDong == id);

            if (hoatDong == null)
            {
                return NotFound();
            }

            var allActiveActivities = await _context.HoatDongs
                .Where(h => h.MaTrangThai == 2) // Lấy tất cả hoạt động đã được xác nhận
                .ToListAsync();

            if (!allActiveActivities.Any(h => h.MaHoatDong == id))
            {
                allActiveActivities.Add(hoatDong);
            }

            var recommendedIds = await _recommenderService.GetRecommendedActivityIdsAsync(hoatDong.MaHoatDong, allActiveActivities, 4);

            var relatedActivities = new List<HoatDong>();
            if (recommendedIds != null && recommendedIds.Any())
            {
                relatedActivities = await _context.HoatDongs
                    .Include(h => h.TrangThaiHoatDong)
                    .Include(h => h.DiaDiem)
                    .Where(h => recommendedIds.Contains(h.MaHoatDong))
                    .ToListAsync();
                    
                relatedActivities = relatedActivities.OrderBy(h => recommendedIds.IndexOf(h.MaHoatDong)).ToList();
            }

            ViewBag.RelatedActivities = relatedActivities;

            return View(hoatDong);
        }

        // GET: HoatDong/Create
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Create()
        {
            ViewBag.TrangThaiHoatDongs = new SelectList(await _context.TrangThaiHoatDongs.ToListAsync(), "MaTrangThai", "TenTrangThai");
            ViewBag.DanhMucHoatDongs = new SelectList(await _context.DanhMucHoatDongs.Where(d => d.TrangThai).ToListAsync(), "MaDanhMuc", "TenDanhMuc");
            ViewBag.LoaiHoatDongs = new SelectList(await _context.LoaiHoatDongs.Where(l => l.TrangThai).ToListAsync(), "MaLoaiHoatDong", "TenLoaiHoatDong");
            ViewBag.DiaDiems = new SelectList(await _context.DiaDiems.Where(d => d.TrangThai).ToListAsync(), "MaDiaDiem", "TenDiaDiem");
            return View();
        }

        // POST: HoatDong/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Create([Bind("TieuDe,MoTa,ThoiGianBatDau,ThoiGianKetThuc,MaDanhMuc,MaLoaiHoatDong,MaDiaDiem,SoLuongToiDa,ChiPhi,YeuCauThamGia,TrangBiCanThiet")] HoatDong hoatDong, bool ignoreWarnings = false)
        {
            // Debug: Log thông tin ModelState
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Console.WriteLine($"ModelState errors: {string.Join(", ", errors)}");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra xung đột lịch trước khi tạo
                    var conflictResult = await _scheduleService.CheckScheduleConflictAsync(hoatDong);
                    
                    // Chỉ chặn nếu có xung đột nghiêm trọng và không bỏ qua cảnh báo
                    if (conflictResult.HasConflict && !ignoreWarnings)
                    {
                        // Lấy gợi ý thời gian thay thế
                        var alternativeSlots = await _scheduleService.GetAlternativeTimeSlotsAsync(hoatDong, 5);
                        
                        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        {
                            return Json(new { 
                                success = false, 
                                message = "Phát hiện xung đột lịch!", 
                                conflicts = conflictResult.ConflictMessages,
                                conflictType = conflictResult.ConflictType.ToString(),
                                conflictingActivities = conflictResult.ConflictingActivities.Select(h => new
                                {
                                    h.MaHoatDong,
                                    h.TieuDe,
                                    h.ThoiGianBatDau,
                                    h.ThoiGianKetThuc,
                                    DiaDiem = h.DiaDiem?.TenDiaDiem,
                                    NguoiTao = h.NguoiTao?.UserName
                                }),
                                alternativeSlots = alternativeSlots.Select(s => new
                                {
                                    startTime = s.StartTime.ToString("yyyy-MM-ddTHH:mm"),
                                    endTime = s.EndTime.ToString("yyyy-MM-ddTHH:mm"),
                                    diaDiemName = s.DiaDiemName,
                                    score = s.Score
                                })
                            });
                        }

                        TempData["ErrorMessage"] = string.Join("; ", conflictResult.ConflictMessages);
                        TempData["AlternativeSlots"] = alternativeSlots;
                        TempData["ShowIgnoreButton"] = true; // Thêm flag để hiển thị nút bỏ qua
                        ViewBag.TrangThaiHoatDongs = new SelectList(await _context.TrangThaiHoatDongs.ToListAsync(), "MaTrangThai", "TenTrangThai");
                        ViewBag.DanhMucHoatDongs = new SelectList(await _context.DanhMucHoatDongs.Where(d => d.TrangThai).ToListAsync(), "MaDanhMuc", "TenDanhMuc");
                        ViewBag.LoaiHoatDongs = new SelectList(await _context.LoaiHoatDongs.Where(l => l.TrangThai).ToListAsync(), "MaLoaiHoatDong", "TenLoaiHoatDong");
                        ViewBag.DiaDiems = new SelectList(await _context.DiaDiems.Where(d => d.TrangThai).ToListAsync(), "MaDiaDiem", "TenDiaDiem");
                        return View(hoatDong);
                    }

                    hoatDong.NguoiTaoId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    hoatDong.NgayTao = DateTime.Now;
                    hoatDong.NgayCapNhat = DateTime.Now;
                    
                    // Mặc định trạng thái "Chờ xác nhận" (MaTrangThai = 1)
                    hoatDong.MaTrangThai = 1;

                    _context.Add(hoatDong);
                    await _context.SaveChangesAsync();

                    // Ghi nhật ký thao tác
                    var nhatKy = new NhatKyThaoTac
                    {
                        MaHoatDong = hoatDong.MaHoatDong,
                        LoaiThaoTac = "Tao",
                        MoTaThaoTac = $"Tạo hoạt động: {hoatDong.TieuDe} (Chờ xác nhận)",
                        NguoiThucHienId = hoatDong.NguoiTaoId,
                        ThoiGianThucHien = DateTime.Now
                    };
                    _context.NhatKyThaoTacs.Add(nhatKy);
                    await _context.SaveChangesAsync();

                    // Gửi thông báo realtime
                    var createdBy = User.Identity?.Name ?? "Người dùng";
                    await _notificationService.SendActivityCreatedNotification(hoatDong, createdBy);

                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = true, message = "Tạo hoạt động thành công!", id = hoatDong.MaHoatDong });
                    }

                    TempData["SuccessMessage"] = $"Hoạt động '{hoatDong.TieuDe}' đã được tạo thành công và đang chờ xác nhận!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating activity: {ex.Message}");
                    TempData["ErrorMessage"] = $"Lỗi khi tạo hoạt động: {ex.Message}";
                }
            }

            ViewBag.TrangThaiHoatDongs = new SelectList(await _context.TrangThaiHoatDongs.ToListAsync(), "MaTrangThai", "TenTrangThai");
            ViewBag.DanhMucHoatDongs = new SelectList(await _context.DanhMucHoatDongs.Where(d => d.TrangThai).ToListAsync(), "MaDanhMuc", "TenDanhMuc");
            ViewBag.LoaiHoatDongs = new SelectList(await _context.LoaiHoatDongs.Where(l => l.TrangThai).ToListAsync(), "MaLoaiHoatDong", "TenLoaiHoatDong");
            ViewBag.DiaDiems = new SelectList(await _context.DiaDiems.Where(d => d.TrangThai).ToListAsync(), "MaDiaDiem", "TenDiaDiem");
            return View(hoatDong);
        }

        // GET: HoatDong/Edit/5
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoatDong = await _context.HoatDongs
                .Include(h => h.TrangThaiHoatDong)
                .Include(h => h.DanhMucHoatDong)
                .Include(h => h.LoaiHoatDong)
                .Include(h => h.DiaDiem)
                .FirstOrDefaultAsync(h => h.MaHoatDong == id);
                
            if (hoatDong == null)
            {
                return NotFound();
            }

            // Kiểm tra quyền chỉnh sửa
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!User.IsInRole("Admin") && hoatDong.NguoiTaoId != currentUserId)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền chỉnh sửa hoạt động này!";
                return RedirectToAction(nameof(Index));
            }

            // Chuyển đổi sang ViewModel
            var viewModel = new EditHoatDongViewModel
            {
                MaHoatDong = hoatDong.MaHoatDong,
                TieuDe = hoatDong.TieuDe,
                MoTa = hoatDong.MoTa,
                ThoiGianBatDau = hoatDong.ThoiGianBatDau,
                ThoiGianKetThuc = hoatDong.ThoiGianKetThuc,
                MaTrangThai = hoatDong.MaTrangThai,
                NguoiTaoId = hoatDong.NguoiTaoId,
                NgayTao = hoatDong.NgayTao,
                NgayCapNhat = hoatDong.NgayCapNhat,
                MaDanhMuc = hoatDong.MaDanhMuc,
                MaLoaiHoatDong = hoatDong.MaLoaiHoatDong,
                MaDiaDiem = hoatDong.MaDiaDiem,
                SoLuongToiDa = hoatDong.SoLuongToiDa,
                ChiPhi = hoatDong.ChiPhi,
                YeuCauThamGia = hoatDong.YeuCauThamGia,
                TrangBiCanThiet = hoatDong.TrangBiCanThiet,
                TenTrangThai = hoatDong.TrangThaiHoatDong?.TenTrangThai,
                TenDanhMuc = hoatDong.DanhMucHoatDong?.TenDanhMuc,
                TenLoaiHoatDong = hoatDong.LoaiHoatDong?.TenLoaiHoatDong,
                TenDiaDiem = hoatDong.DiaDiem?.TenDiaDiem
            };

            ViewBag.TrangThaiHoatDongs = new SelectList(await _context.TrangThaiHoatDongs.ToListAsync(), "MaTrangThai", "TenTrangThai");
            ViewBag.DanhMucHoatDongs = await _context.DanhMucHoatDongs.Where(d => d.TrangThai).ToListAsync();
            ViewBag.LoaiHoatDongs = await _context.LoaiHoatDongs.Where(l => l.TrangThai).ToListAsync();
            ViewBag.DiaDiems = await _context.DiaDiems.Where(d => d.TrangThai).ToListAsync();
            return View(viewModel);
        }

        // POST: HoatDong/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Edit(int id, EditHoatDongViewModel viewModel)
        {
            if (id != viewModel.MaHoatDong)
            {
                return NotFound();
            }

            // Kiểm tra quyền chỉnh sửa
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var existingHoatDong = await _context.HoatDongs.FindAsync(id);
            
            if (existingHoatDong == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy hoạt động!";
                return RedirectToAction(nameof(Index));
            }
            
            if (!User.IsInRole("Admin") && existingHoatDong.NguoiTaoId != currentUserId)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền chỉnh sửa hoạt động này!";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                // Validation thời gian
                if (viewModel.ThoiGianKetThuc <= viewModel.ThoiGianBatDau)
                {
                    ModelState.AddModelError("ThoiGianKetThuc", "Thời gian kết thúc phải sau thời gian bắt đầu!");
                    ViewBag.TrangThaiHoatDongs = new SelectList(await _context.TrangThaiHoatDongs.ToListAsync(), "MaTrangThai", "TenTrangThai");
                    ViewBag.DanhMucHoatDongs = await _context.DanhMucHoatDongs.Where(d => d.TrangThai).ToListAsync();
                    ViewBag.LoaiHoatDongs = await _context.LoaiHoatDongs.Where(l => l.TrangThai).ToListAsync();
                    ViewBag.DiaDiems = await _context.DiaDiems.Where(d => d.TrangThai).ToListAsync();
                    return View(viewModel);
                }

                if (viewModel.ThoiGianBatDau < DateTime.Now)
                {
                    ModelState.AddModelError("ThoiGianBatDau", "Thời gian bắt đầu không thể trong quá khứ!");
                    ViewBag.TrangThaiHoatDongs = new SelectList(await _context.TrangThaiHoatDongs.ToListAsync(), "MaTrangThai", "TenTrangThai");
                    ViewBag.DanhMucHoatDongs = await _context.DanhMucHoatDongs.Where(d => d.TrangThai).ToListAsync();
                    ViewBag.LoaiHoatDongs = await _context.LoaiHoatDongs.Where(l => l.TrangThai).ToListAsync();
                    ViewBag.DiaDiems = await _context.DiaDiems.Where(d => d.TrangThai).ToListAsync();
                    return View(viewModel);
                }

                try
                {
                    // Debug logging
                    Console.WriteLine($"Updating activity {existingHoatDong.MaHoatDong}");
                    Console.WriteLine($"New title: {viewModel.TieuDe}");
                    Console.WriteLine($"New status: {viewModel.MaTrangThai}");
                    Console.WriteLine($"New start time: {viewModel.ThoiGianBatDau}");
                    Console.WriteLine($"New end time: {viewModel.ThoiGianKetThuc}");
                    
                    // Chỉ cập nhật các field có trong form Edit
                    existingHoatDong.TieuDe = viewModel.TieuDe;
                    existingHoatDong.MoTa = viewModel.MoTa;
                    existingHoatDong.ThoiGianBatDau = viewModel.ThoiGianBatDau;
                    existingHoatDong.ThoiGianKetThuc = viewModel.ThoiGianKetThuc;
                    existingHoatDong.MaTrangThai = viewModel.MaTrangThai;
                    existingHoatDong.NgayCapNhat = DateTime.Now;
                    
                    Console.WriteLine("About to save changes...");
                    await _context.SaveChangesAsync();
                    Console.WriteLine("Changes saved successfully!");

                    // Ghi nhật ký thao tác
                    var nhatKy = new NhatKyThaoTac
                    {
                        MaHoatDong = existingHoatDong.MaHoatDong,
                        LoaiThaoTac = "Sua",
                        MoTaThaoTac = $"Cập nhật hoạt động: {existingHoatDong.TieuDe}",
                        NguoiThucHienId = currentUserId,
                        ThoiGianThucHien = DateTime.Now
                    };
                    _context.NhatKyThaoTacs.Add(nhatKy);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"Đã cập nhật hoạt động '{existingHoatDong.TieuDe}' thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HoatDongExists(viewModel.MaHoatDong))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi khi cập nhật hoạt động: {ex.Message}";
                    ViewBag.TrangThaiHoatDongs = new SelectList(await _context.TrangThaiHoatDongs.ToListAsync(), "MaTrangThai", "TenTrangThai");
                    ViewBag.DanhMucHoatDongs = await _context.DanhMucHoatDongs.Where(d => d.TrangThai).ToListAsync();
                    ViewBag.LoaiHoatDongs = await _context.LoaiHoatDongs.Where(l => l.TrangThai).ToListAsync();
                    ViewBag.DiaDiems = await _context.DiaDiems.Where(d => d.TrangThai).ToListAsync();
                    return View(viewModel);
                }
            }

            // Nếu ModelState không hợp lệ, load lại ViewBag và trả về View
            ViewBag.TrangThaiHoatDongs = new SelectList(await _context.TrangThaiHoatDongs.ToListAsync(), "MaTrangThai", "TenTrangThai");
            ViewBag.DanhMucHoatDongs = await _context.DanhMucHoatDongs.Where(d => d.TrangThai).ToListAsync();
            ViewBag.LoaiHoatDongs = await _context.LoaiHoatDongs.Where(l => l.TrangThai).ToListAsync();
            ViewBag.DiaDiems = await _context.DiaDiems.Where(d => d.TrangThai).ToListAsync();
            return View(viewModel);
        }

        // GET: HoatDong/Delete/5
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoatDong = await _context.HoatDongs
                .Include(h => h.TrangThaiHoatDong)
                .Include(h => h.DanhMucHoatDong)
                .Include(h => h.LoaiHoatDong)
                .Include(h => h.DiaDiem)
                .Include(h => h.NguoiTao)
                .Include(h => h.DanhSachDangKy)
                .FirstOrDefaultAsync(m => m.MaHoatDong == id);

            if (hoatDong == null)
            {
                return NotFound();
            }

            // Kiểm tra quyền xóa
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!User.IsInRole("Admin") && hoatDong.NguoiTaoId != currentUserId)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền xóa hoạt động này!";
                return RedirectToAction(nameof(Index));
            }

            return View(hoatDong);
        }

        // POST: HoatDong/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var hoatDong = await _context.HoatDongs
                    .Include(h => h.DanhSachDangKy)
                    .Include(h => h.DanhSachFile)
                    .Include(h => h.DanhSachThongBao)
                    .FirstOrDefaultAsync(h => h.MaHoatDong == id);

                if (hoatDong == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy hoạt động!";
                    return RedirectToAction(nameof(Index));
                }

                // Kiểm tra quyền xóa
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!User.IsInRole("Admin") && hoatDong.NguoiTaoId != currentUserId)
                {
                    TempData["ErrorMessage"] = "Bạn không có quyền xóa hoạt động này!";
                    return RedirectToAction(nameof(Index));
                }

                // Xóa tất cả đăng ký tham gia trước khi xóa hoạt động
                var dangKys = await _context.DangKyThamGias
                    .Where(d => d.MaHoatDong == id)
                    .ToListAsync();
                if (dangKys.Any())
                {
                    _context.DangKyThamGias.RemoveRange(dangKys);
                }

                // Xóa các bản ghi liên quan trước
                if (hoatDong.DanhSachFile?.Any() == true)
                {
                    _context.FileDinhKems.RemoveRange(hoatDong.DanhSachFile);
                }

                if (hoatDong.DanhSachThongBao?.Any() == true)
                {
                    _context.ThongBaos.RemoveRange(hoatDong.DanhSachThongBao);
                }

                // Xóa nhật ký thao tác liên quan
                var nhatKyThaoTacs = await _context.NhatKyThaoTacs
                    .Where(n => n.MaHoatDong == id)
                    .ToListAsync();
                if (nhatKyThaoTacs.Any())
                {
                    _context.NhatKyThaoTacs.RemoveRange(nhatKyThaoTacs);
                }

                // Xóa hoạt động
                _context.HoatDongs.Remove(hoatDong);
                await _context.SaveChangesAsync();

                var dangKyCount = dangKys.Count;
                if (dangKyCount > 0)
                {
                    TempData["SuccessMessage"] = $"Đã xóa hoạt động '{hoatDong.TieuDe}' và {dangKyCount} đăng ký tham gia thành công!";
                }
                else
                {
                    TempData["SuccessMessage"] = $"Đã xóa hoạt động '{hoatDong.TieuDe}' thành công!";
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting activity: {ex.Message}");
                TempData["ErrorMessage"] = $"Lỗi khi xóa hoạt động: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }



        // POST: HoatDong/Hide/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> HideActivity(int id)
        {
            try
            {
                var hoatDong = await _context.HoatDongs.FindAsync(id);
                if (hoatDong == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy hoạt động!";
                    return RedirectToAction(nameof(Index));
                }

                // Kiểm tra quyền
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!User.IsInRole("Admin") && hoatDong.NguoiTaoId != currentUserId)
                {
                    TempData["ErrorMessage"] = "Bạn không có quyền ẩn hoạt động này!";
                    return RedirectToAction(nameof(Index));
                }

                // Đánh dấu hoạt động là đã ẩn (thay đổi trạng thái)
                hoatDong.MaTrangThai = 4; // Giả sử trạng thái 4 là "Đã ẩn"
                hoatDong.NgayCapNhat = DateTime.Now;
                await _context.SaveChangesAsync();

                // Ghi nhật ký thao tác
                var nhatKy = new NhatKyThaoTac
                {
                    MaHoatDong = hoatDong.MaHoatDong,
                    LoaiThaoTac = "An",
                    MoTaThaoTac = $"Ẩn hoạt động: {hoatDong.TieuDe}",
                    NguoiThucHienId = currentUserId,
                    ThoiGianThucHien = DateTime.Now
                };
                _context.NhatKyThaoTacs.Add(nhatKy);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Đã ẩn hoạt động '{hoatDong.TieuDe}' thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error hiding activity: {ex.Message}");
                TempData["ErrorMessage"] = $"Lỗi khi ẩn hoạt động: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: HoatDong/XacNhanHoatDong
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> XacNhanHoatDong()
        {
            var hoatDongsChoXacNhan = await _context.HoatDongs
                .Include(h => h.TrangThaiHoatDong)
                .Include(h => h.DanhMucHoatDong)
                .Include(h => h.LoaiHoatDong)
                .Include(h => h.DiaDiem)
                .Where(h => h.MaTrangThai == 1) // Chỉ lấy hoạt động chờ xác nhận
                .OrderByDescending(h => h.NgayTao)
                .ToListAsync();

            return View(hoatDongsChoXacNhan);
        }

        // POST: HoatDong/XacNhanHoatDong
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> XacNhanHoatDong(int maHoatDong, bool dongY)
        {
            try
            {
                var hoatDong = await _context.HoatDongs.FindAsync(maHoatDong);
                if (hoatDong == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy hoạt động!";
                    return RedirectToAction(nameof(XacNhanHoatDong));
                }

                if (dongY)
                {
                    hoatDong.MaTrangThai = 2; // Đã xác nhận
                    TempData["SuccessMessage"] = $"Đã xác nhận hoạt động: {hoatDong.TieuDe}";
                }
                else
                {
                    hoatDong.MaTrangThai = 3; // Từ chối
                    TempData["SuccessMessage"] = $"Đã từ chối hoạt động: {hoatDong.TieuDe}";
                }

                hoatDong.NgayCapNhat = DateTime.Now;
                await _context.SaveChangesAsync();

                // Ghi nhật ký thao tác
                var nhatKy = new NhatKyThaoTac
                {
                    MaHoatDong = hoatDong.MaHoatDong,
                    LoaiThaoTac = dongY ? "XacNhan" : "TuChoi",
                    MoTaThaoTac = dongY ? $"Xác nhận hoạt động: {hoatDong.TieuDe}" : $"Từ chối hoạt động: {hoatDong.TieuDe}",
                    NguoiThucHienId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                    ThoiGianThucHien = DateTime.Now
                };

                _context.NhatKyThaoTacs.Add(nhatKy);
                await _context.SaveChangesAsync();

                // Gửi thông báo realtime
                var status = dongY ? "xác nhận" : "từ chối";
                await _notificationService.SendActivityConfirmationNotification(hoatDong, status);

                var message = dongY ? $"Đã xác nhận hoạt động: {hoatDong.TieuDe}" : $"Đã từ chối hoạt động: {hoatDong.TieuDe}";

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true, message = message, id = hoatDong.MaHoatDong });
                }

                TempData["SuccessMessage"] = message;
                return RedirectToAction(nameof(XacNhanHoatDong));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                return RedirectToAction(nameof(XacNhanHoatDong));
            }
        }

        // GET: HoatDong/GetActivityList (cho AJAX)
        [HttpGet]
        public async Task<IActionResult> GetActivityList()
        {
            var hoatDongs = await _context.HoatDongs
                .Include(h => h.TrangThaiHoatDong)
                .Include(h => h.DanhMucHoatDong)
                .Include(h => h.LoaiHoatDong)
                .Include(h => h.DiaDiem)
                .Include(h => h.NguoiTao)
                .Include(h => h.DanhSachDangKy)
                .ThenInclude(d => d.TrangThaiDangKy)
                .Include(h => h.DanhSachDangKy)
                .ThenInclude(d => d.User)
                .OrderByDescending(h => h.NgayTao)
                .ToListAsync();

            return PartialView("_ActivityList", hoatDongs);
        }

        // GET: HoatDong/GetStatistics (cho AJAX)
        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            var totalActivities = await _context.HoatDongs.CountAsync();
            var conflictCount = await _context.HoatDongs
                .Where(h => h.MaTrangThai == 2) // Giả sử trạng thái 2 là có xung đột
                .CountAsync();
            var totalRegistrations = await _context.DangKyThamGias.CountAsync();

            return Json(new
            {
                totalActivities,
                conflictCount,
                totalRegistrations
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActivitiesForCalendar()
        {
            try
            {
                var activities = await _context.HoatDongs
                    .Include(h => h.TrangThaiHoatDong)
                    .Include(h => h.DanhMucHoatDong)
                    .Include(h => h.DiaDiem)
                    .Include(h => h.NguoiTao)
                    .OrderByDescending(h => h.NgayTao)
                    .Select(h => new
                    {
                        maHoatDong = h.MaHoatDong,
                        tieuDe = h.TieuDe,
                        thoiGianBatDau = h.ThoiGianBatDau,
                        thoiGianKetThuc = h.ThoiGianKetThuc,
                        moTa = h.MoTa,
                        nguoiTaoId = h.NguoiTaoId,
                        nguoiTao = h.NguoiTao != null ? new
                        {
                            userName = h.NguoiTao.UserName
                        } : null,
                        trangThaiHoatDong = h.TrangThaiHoatDong != null ? h.TrangThaiHoatDong.TenTrangThai : null,
                        diaDiem = h.DiaDiem != null ? h.DiaDiem.TenDiaDiem : null,
                        danhMuc = h.DanhMucHoatDong != null ? h.DanhMucHoatDong.TenDanhMuc : null
                    })
                    .ToListAsync();

                return Json(activities);
            }
            catch (Exception ex)
            {
                return Json(new List<object>());
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> CheckConflict([FromBody] HoatDong hoatDong)
        {
            try
            {
                var conflictResult = await _scheduleService.CheckScheduleConflictAsync(hoatDong);
                var alternativeSlots = await _scheduleService.GetAlternativeTimeSlotsAsync(hoatDong, 5);

                return Json(new
                {
                    hasConflict = conflictResult.HasConflict,
                    conflicts = conflictResult.ConflictMessages,
                    conflictType = conflictResult.ConflictType.ToString(),
                    conflictingActivities = conflictResult.ConflictingActivities.Select(h => new
                    {
                        h.MaHoatDong,
                        h.TieuDe,
                        h.ThoiGianBatDau,
                        h.ThoiGianKetThuc,
                        DiaDiem = h.DiaDiem?.TenDiaDiem,
                        NguoiTao = h.NguoiTao?.UserName
                    }),
                    alternativeSlots = alternativeSlots.Select(s => new
                    {
                        startTime = s.StartTime.ToString("yyyy-MM-ddTHH:mm"),
                        endTime = s.EndTime.ToString("yyyy-MM-ddTHH:mm"),
                        diaDiemName = s.DiaDiemName,
                        score = s.Score
                    })
                });
            }
            catch (Exception ex)
            {
                return Json(new { hasConflict = false, conflicts = new List<string>(), alternativeSlots = new List<object>() });
            }
        }

        private bool HoatDongExists(int id)
        {
            return _context.HoatDongs.Any(e => e.MaHoatDong == id);
        }
    }
}
