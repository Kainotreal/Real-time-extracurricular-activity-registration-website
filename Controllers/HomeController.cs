using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyHoatDongNgoaiKhoa.Models;
using System.Security.Claims;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var dashboardData = new DashboardViewModel
        {
            TotalHoatDong = await _context.HoatDongs.CountAsync(),
            TotalNguoiDung = 0, // Sẽ lấy từ Identity sau
            TotalDangKy = await _context.DangKyThamGias.CountAsync(),
            HoatDongDangDienRa = await _context.HoatDongs
                .Where(h => h.ThoiGianBatDau <= DateTime.Now && h.ThoiGianKetThuc >= DateTime.Now)
                .CountAsync(),
            HoatDongSapDienRa = await _context.HoatDongs
                .Where(h => h.ThoiGianBatDau > DateTime.Now)
                .CountAsync(),
            HoatDongDaKetThuc = await _context.HoatDongs
                .Where(h => h.ThoiGianKetThuc < DateTime.Now)
                .CountAsync()
        };

        // Lấy hoạt động gần đây
        dashboardData.HoatDongGanDay = await _context.HoatDongs
            .Include(h => h.TrangThaiHoatDong)
            .Include(h => h.NguoiTao)
            .OrderByDescending(h => h.NgayTao)
            .Take(5)
            .ToListAsync();

        // Lấy đăng ký gần đây
        dashboardData.DangKyGanDay = await _context.DangKyThamGias
            .Include(d => d.HoatDong)
            .Include(d => d.TrangThaiDangKy)
            .Include(d => d.User)
            .OrderByDescending(d => d.NgayDangKy)
            .Take(10)
            .ToListAsync();

        return View(dashboardData);
    }
}

public class DashboardViewModel
{
    public int TotalHoatDong { get; set; }
    public int TotalNguoiDung { get; set; }
    public int TotalDangKy { get; set; }
    public int HoatDongDangDienRa { get; set; }
    public int HoatDongSapDienRa { get; set; }
    public int HoatDongDaKetThuc { get; set; }
    public List<HoatDong> HoatDongGanDay { get; set; } = new List<HoatDong>();
    public List<DangKyThamGia> DangKyGanDay { get; set; } = new List<DangKyThamGia>();
}