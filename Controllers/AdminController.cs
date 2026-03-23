using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyHoatDongNgoaiKhoa.Models;
using QuanLyHoatDongNgoaiKhoa.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using QuanLyHoatDongNgoaiKhoa.Services;

namespace QuanLyHoatDongNgoaiKhoa.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly INotificationService _notificationService;

        public AdminController(AppDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, INotificationService notificationService)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _notificationService = notificationService;
        }

        // GET: Admin
        public async Task<IActionResult> Index()
        {
            // Lấy thống kê tổng quan
            ViewBag.TotalUsers = await _userManager.Users.CountAsync();
            ViewBag.TotalActivities = await _context.HoatDongs.CountAsync();
            ViewBag.TotalRegistrations = await _context.DangKyThamGias.CountAsync();
            ViewBag.PendingRegistrations = await _context.DangKyThamGias
                .Where(d => d.MaTrangThaiDangKy == 1) // 1 = "Chờ xác nhận"
                .CountAsync();

            // Lấy hoạt động gần đây
            var recentActivities = await _context.NhatKyThaoTacs
                .Include(n => n.HoatDong)
                .OrderByDescending(n => n.ThoiGianThucHien)
                .Take(5)
                .ToListAsync();

            var recentActivitiesViewModel = recentActivities.Select(n => new
            {
                Icon = GetActivityIcon(n.LoaiThaoTac),
                Title = GetActivityTitle(n.LoaiThaoTac),
                Description = n.MoTaThaoTac,
                Time = n.ThoiGianThucHien
            }).ToList();

            ViewBag.RecentActivities = recentActivitiesViewModel;

            return View();
        }

        // GET: Admin/Users
        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            var userViewModels = new List<UserManagementViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userViewModels.Add(new UserManagementViewModel
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    UserName = user.UserName ?? string.Empty,
                    CurrentRoles = roles.ToList(),
                    IsLocked = user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.Now
                });
            }

            return View(userViewModels);
        }

        // GET: Admin/EditUser/{id}
        public async Task<IActionResult> EditUser(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy người dùng!";
                    return RedirectToAction(nameof(Users));
                }

                var userRoles = await _userManager.GetRolesAsync(user);
                var allRoles = await _roleManager.Roles.ToListAsync();

                var viewModel = new UserEditViewModel
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    UserName = user.UserName ?? string.Empty,
                    CurrentRoles = userRoles.Distinct().ToList(), // Loại bỏ trùng lặp
                    AvailableRoles = allRoles.Select(r => r.Name ?? string.Empty).Distinct().ToList(), // Loại bỏ trùng lặp
                    IsLocked = user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.Now
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                return RedirectToAction(nameof(Users));
            }
        }

        // POST: Admin/EditUser
        [HttpPost]
        public async Task<IActionResult> EditUser(UserEditViewModel model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy người dùng!";
                    return RedirectToAction(nameof(Users));
                }

                // Lấy thông tin người dùng hiện tại đang thực hiện hành động
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var currentUser = await _userManager.FindByIdAsync(currentUserId);
                
                // Kiểm tra vai trò hiện tại của người dùng được chỉnh sửa
                var userCurrentRoles = await _userManager.GetRolesAsync(user);
                var currentUserRoles = await _userManager.GetRolesAsync(currentUser);

                // Ngăn chặn Admin hạ cấp Admin khác hoặc tự hạ cấp chính mình
                if (userCurrentRoles.Contains("Admin") && currentUserRoles.Contains("Admin"))
                {
                    // Kiểm tra xem có đang cố gắng loại bỏ vai trò Admin không
                    var isRemovingAdminRole = userCurrentRoles.Contains("Admin") && !model.SelectedRoles.Contains("Admin");
                    
                    if (isRemovingAdminRole)
                    {
                        // Kiểm tra xem có phải đang chỉnh sửa chính mình không
                        var isEditingSelf = model.Id == currentUserId;
                        
                        if (isEditingSelf)
                        {
                            TempData["ErrorMessage"] = "Không thể tự hạ cấp chính mình! Bạn phải giữ vai trò Admin để quản lý hệ thống.";
                            return RedirectToAction(nameof(EditUser), new { id = model.Id });
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Không thể hạ cấp quản trị viên khác! Chỉ có thể thay đổi vai trò của người dùng không phải Admin.";
                            return RedirectToAction(nameof(EditUser), new { id = model.Id });
                        }
                    }
                }

                // Cập nhật thông tin cơ bản
                user.Email = model.Email;
                user.UserName = model.UserName;
                var updateResult = await _userManager.UpdateAsync(user);

                if (!updateResult.Succeeded)
                {
                    TempData["ErrorMessage"] = "Lỗi khi cập nhật thông tin người dùng!";
                    return RedirectToAction(nameof(EditUser), new { id = model.Id });
                }

                // Cập nhật vai trò
                var currentRoles = await _userManager.GetRolesAsync(user);
                var rolesToRemove = currentRoles.Except(model.SelectedRoles);
                var rolesToAdd = model.SelectedRoles.Except(currentRoles);

                if (rolesToRemove.Any())
                {
                    await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                }

                if (rolesToAdd.Any())
                {
                    await _userManager.AddToRolesAsync(user, rolesToAdd);
                }

                TempData["SuccessMessage"] = "Đã cập nhật thông tin người dùng thành công!";
                return RedirectToAction(nameof(Users));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                return RedirectToAction(nameof(EditUser), new { id = model.Id });
            }
        }

        // POST: Admin/LockUser
        [HttpPost]
        public async Task<IActionResult> LockUser(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy người dùng!";
                    return RedirectToAction(nameof(Users));
                }

                // Lấy thông tin người dùng hiện tại đang thực hiện hành động
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(currentUserId))
                {
                    TempData["ErrorMessage"] = "Không tìm thấy thông tin người dùng!";
                    return RedirectToAction(nameof(Users));
                }
                var currentUser = await _userManager.FindByIdAsync(currentUserId);
                if (currentUser == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy người dùng hiện tại!";
                    return RedirectToAction(nameof(Users));
                }
                
                // Kiểm tra vai trò của người dùng được khóa
                var userRoles = await _userManager.GetRolesAsync(user);
                var currentUserRoles = await _userManager.GetRolesAsync(currentUser);

                // Ngăn chặn Admin khóa Admin khác
                if (userRoles.Contains("Admin") && currentUserRoles.Contains("Admin"))
                {
                    TempData["ErrorMessage"] = "Không thể khóa quản trị viên khác! Chỉ có thể khóa người dùng không phải Admin.";
                    return RedirectToAction(nameof(Users));
                }

                // Khóa tài khoản trong 100 năm (hiệu quả như vô thời hạn)
                var result = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.Now.AddYears(100));
                
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Đã khóa tài khoản thành công!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Lỗi khi khóa tài khoản!";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
            }

            return RedirectToAction(nameof(Users));
        }

        // POST: Admin/UnlockUser
        [HttpPost]
        public async Task<IActionResult> UnlockUser(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy người dùng!";
                    return RedirectToAction(nameof(Users));
                }

                var result = await _userManager.SetLockoutEndDateAsync(user, null);
                
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Đã mở khóa tài khoản thành công!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Lỗi khi mở khóa tài khoản!";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
            }

            return RedirectToAction(nameof(Users));
        }

        // POST: Admin/DeleteUser
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
                {
                    try
                    {
                        var user = await _userManager.FindByIdAsync(id);
                        if (user == null)
                        {
                            TempData["ErrorMessage"] = "Không tìm thấy người dùng!";
                            return RedirectToAction(nameof(Users));
                        }

                        // Lấy thông tin người dùng hiện tại đang thực hiện hành động
                        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        if (string.IsNullOrEmpty(currentUserId))
                        {
                            TempData["ErrorMessage"] = "Không tìm thấy thông tin người dùng!";
                            return RedirectToAction(nameof(Users));
                        }
                        var currentUser = await _userManager.FindByIdAsync(currentUserId);
                        if (currentUser == null)
                        {
                            TempData["ErrorMessage"] = "Không tìm thấy người dùng hiện tại!";
                            return RedirectToAction(nameof(Users));
                        }
                
                // Kiểm tra vai trò của người dùng được xóa
                var userRoles = await _userManager.GetRolesAsync(user);
                var currentUserRoles = await _userManager.GetRolesAsync(currentUser);

                // Ngăn chặn Admin xóa Admin khác
                if (userRoles.Contains("Admin") && currentUserRoles.Contains("Admin"))
                {
                    TempData["ErrorMessage"] = "Không thể xóa quản trị viên khác! Chỉ có thể xóa người dùng không phải Admin.";
                    return RedirectToAction(nameof(Users));
                }

                var result = await _userManager.DeleteAsync(user);
                
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Đã xóa tài khoản thành công!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Lỗi khi xóa tài khoản!";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
            }

            return RedirectToAction(nameof(Users));
        }

        // GET: Admin/Reports
        public async Task<IActionResult> Reports()
        {
            // Thống kê theo tháng
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            ViewBag.MonthlyStats = new
            {
                ActivitiesThisMonth = await _context.HoatDongs
                    .Where(h => h.NgayTao.Month == currentMonth && h.NgayTao.Year == currentYear)
                    .CountAsync(),
                RegistrationsThisMonth = await _context.DangKyThamGias
                    .Where(d => d.NgayDangKy.Month == currentMonth && d.NgayDangKy.Year == currentYear)
                    .CountAsync(),
                TotalUsers = await _userManager.Users.CountAsync()
            };

            // Thống kê theo danh mục hoạt động
            ViewBag.CategoryStats = await _context.HoatDongs
                .Include(h => h.DanhMucHoatDong)
                .GroupBy(h => h.DanhMucHoatDong.TenDanhMuc)
                .Select(g => new
                {
                    Category = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            // Thống kê trạng thái đăng ký
            ViewBag.RegistrationStatusStats = await _context.DangKyThamGias
                .Include(d => d.TrangThaiDangKy)
                .GroupBy(d => d.TrangThaiDangKy.TenTrangThaiDangKy)
                .Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            return View();
        }

        // GET: Admin/GetStatistics (cho AJAX)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetStatistics()
        {
            var stats = new
            {
                totalActivities = await _context.HoatDongs.CountAsync(),
                totalRegistrations = await _context.DangKyThamGias.CountAsync(),
                pendingRegistrations = await _context.DangKyThamGias
                    .Where(d => d.MaTrangThaiDangKy == 1)
                    .CountAsync()
            };

            return Json(stats);
        }

        // GET: Admin/CleanupDuplicateRoles
        public async Task<IActionResult> CleanupDuplicateRoles()
        {
            try
            {
                var allRoles = await _roleManager.Roles.ToListAsync();
                var uniqueRoleNames = allRoles.Select(r => r.Name).Distinct().ToList();
                var removedCount = 0;

                foreach (var roleName in uniqueRoleNames)
                {
                    var duplicateRoles = allRoles.Where(r => r.Name == roleName).Skip(1).ToList();
                    foreach (var duplicateRole in duplicateRoles)
                    {
                        var result = await _roleManager.DeleteAsync(duplicateRole);
                        if (result.Succeeded)
                        {
                            removedCount++;
                        }
                    }
                }

                if (removedCount > 0)
                {
                    TempData["SuccessMessage"] = $"Đã dọn dẹp {removedCount} vai trò trùng lặp!";
                }
                else
                {
                    TempData["InfoMessage"] = "Không có vai trò trùng lặp nào cần dọn dẹp!";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi dọn dẹp vai trò: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/CreateRoles
        public async Task<IActionResult> CreateRoles()
        {
            try
            {
                // Tạo các vai trò cơ bản nếu chưa có
                var roles = new[] { "Admin", "Teacher", "Student" };
                var createdRoles = new List<string>();
                
                foreach (var roleName in roles)
                {
                    if (!await _roleManager.RoleExistsAsync(roleName))
                    {
                        var newRole = new IdentityRole(roleName);
                        var result = await _roleManager.CreateAsync(newRole);
                        if (result.Succeeded)
                        {
                            createdRoles.Add(roleName);
                        }
                    }
                }

                if (createdRoles.Any())
                {
                    TempData["SuccessMessage"] = $"Đã tạo các vai trò: {string.Join(", ", createdRoles)}";
                }
                else
                {
                    TempData["InfoMessage"] = "Tất cả các vai trò đã tồn tại!";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi tạo vai trò: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/CheckRoles
        public async Task<IActionResult> CheckRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = roles;
            return View();
        }

        // GET: Admin/Notifications
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Notifications()
        {
            var thongBaos = await _context.ThongBaos
                .Include(t => t.HoatDong)
                .OrderByDescending(t => t.NgayTao)
                .Take(50) // Lấy 50 thông báo gần nhất
                .ToListAsync();

            return View(thongBaos);
        }

        // GET: Admin/MyNotifications (cho tất cả user xem thông báo của họ)
        [Authorize]
        public async Task<IActionResult> MyNotifications()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
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
                // Teacher/Student: Chỉ xem thông báo liên quan đến hoạt động của mình
                thongBaosQuery = _context.ThongBaos
                    .Include(t => t.HoatDong)
                    .ThenInclude(h => h.NguoiTao)
                    .Where(t => 
                        // Thông báo từ hoạt động mà user đã đăng ký
                        t.HoatDong.DanhSachDangKy.Any(d => d.UserId == userId) ||
                        // Thông báo từ hoạt động mà user tạo (cho Teacher)
                        (userRoles.Contains("Teacher") && t.HoatDong.NguoiTaoId == userId) ||
                        // Thông báo gửi trực tiếp cho user
                        t.NguoiGuiId == userId
                    )
                    .OrderByDescending(t => t.NgayTao);
            }

            var thongBaos = await thongBaosQuery
                .Take(50) // Lấy 50 thông báo gần nhất
                .ToListAsync();

            return View("MyNotifications", thongBaos);
        }

        // POST: Admin/MarkNotificationAsRead
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MarkNotificationAsRead(int id)
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



        private string GetActivityIcon(string loaiThaoTac)
        {
            return loaiThaoTac switch
            {
                "Tao" => "plus-circle",
                "Sua" => "edit",
                "Xoa" => "trash",
                "DangKy" => "user-plus",
                "XacNhanDangKy" => "check-circle",
                "TuChoiDangKy" => "times-circle",
                "HuyDangKy" => "user-minus",
                _ => "info-circle"
            };
        }

        private string GetActivityTitle(string loaiThaoTac)
        {
            return loaiThaoTac switch
            {
                "Tao" => "Tạo hoạt động mới",
                "Sua" => "Cập nhật hoạt động",
                "Xoa" => "Xóa hoạt động",
                "DangKy" => "Đăng ký tham gia",
                "XacNhanDangKy" => "Xác nhận đăng ký",
                "TuChoiDangKy" => "Từ chối đăng ký",
                "HuyDangKy" => "Hủy đăng ký",
                _ => "Hoạt động khác"
            };
        }
    }
} 