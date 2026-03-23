using Microsoft.EntityFrameworkCore;
using QuanLyHoatDongNgoaiKhoa.Models;

namespace QuanLyHoatDongNgoaiKhoa.Services
{
    public class ScheduleOptimizationService : IScheduleOptimizationService
    {
        private readonly AppDbContext _context;

        public ScheduleOptimizationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ScheduleConflictResult> CheckScheduleConflictAsync(HoatDong hoatDong, int? excludeHoatDongId = null)
        {
            var result = new ScheduleConflictResult();

            // Kiểm tra nếu hoatDong có đủ thông tin cần thiết
            if (hoatDong.MaDiaDiem == 0 || hoatDong.ThoiGianBatDau == default || hoatDong.ThoiGianKetThuc == default)
            {
                return result; // Không kiểm tra nếu thiếu thông tin
            }

            try
            {
                // 1. Kiểm tra xung đột thời gian và địa điểm
                var query = _context.HoatDongs
                    .Include(h => h.DiaDiem)
                    .Include(h => h.TrangThaiHoatDong)
                    .Include(h => h.NguoiTao)
                    .Where(h => h.MaDiaDiem == hoatDong.MaDiaDiem &&
                               h.MaTrangThai != 3 && // Không phải trạng thái từ chối
                               h.MaHoatDong != excludeHoatDongId);

                // Kiểm tra xung đột thời gian chính xác
                var timeConflicts = await query
                    .Where(h => (h.ThoiGianBatDau < hoatDong.ThoiGianKetThuc && 
                               h.ThoiGianKetThuc > hoatDong.ThoiGianBatDau))
                    .ToListAsync();

                if (timeConflicts.Any())
                {
                    result.HasConflict = true;
                    result.ConflictType = ConflictType.TimeOverlap;
                    result.ConflictingActivities.AddRange(timeConflicts);
                    result.ConflictMessages.Add($"⚠️ Xung đột thời gian với {timeConflicts.Count} hoạt động khác tại địa điểm này.");
                }

                // 2. Kiểm tra giới hạn hoạt động của người tạo (tối đa 5 hoạt động/ngày)
                if (!string.IsNullOrEmpty(hoatDong.NguoiTaoId))
                {
                    var userDailyLimit = await _context.HoatDongs
                        .Where(h => h.NguoiTaoId == hoatDong.NguoiTaoId &&
                                   h.MaHoatDong != excludeHoatDongId &&
                                   h.MaTrangThai != 3 &&
                                   h.ThoiGianBatDau.Date == hoatDong.ThoiGianBatDau.Date)
                        .CountAsync();

                    if (userDailyLimit >= 5)
                    {
                        result.HasConflict = true;
                        result.ConflictType = ConflictType.UserLimitExceeded;
                        result.ConflictMessages.Add($"🚫 Bạn đã tạo {userDailyLimit} hoạt động trong ngày này. Giới hạn tối đa là 5 hoạt động/ngày.");
                    }
                }
            }
            catch (Exception ex)
            {
                // Log lỗi nhưng không chặn tạo hoạt động
                Console.WriteLine($"Error in CheckScheduleConflictAsync: {ex.Message}");
            }

            return result;
        }

        public async Task<ScheduleConflictResult> CheckUserScheduleConflictAsync(string userId, HoatDong hoatDong, int? excludeHoatDongId = null)
        {
            var result = new ScheduleConflictResult();

            // Lấy danh sách hoạt động mà người dùng đã đăng ký
            var userRegistrations = await _context.DangKyThamGias
                .Include(d => d.HoatDong)
                .ThenInclude(h => h.DiaDiem)
                .Where(d => d.UserId == userId &&
                           d.MaTrangThaiDangKy != 3 && // Không phải trạng thái từ chối
                           d.HoatDong != null &&
                           d.HoatDong.MaTrangThai != 3) // Hoạt động không bị từ chối
                .ToListAsync();

            var conflicts = userRegistrations
                .Where(d => d.HoatDong != null && 
                           d.HoatDong.MaHoatDong != excludeHoatDongId &&
                           (d.HoatDong.ThoiGianBatDau < hoatDong.ThoiGianKetThuc && 
                            d.HoatDong.ThoiGianKetThuc > hoatDong.ThoiGianBatDau))
                .Select(d => d.HoatDong!)
                .ToList();

            if (conflicts.Any())
            {
                result.HasConflict = true;
                result.ConflictType = ConflictType.UserScheduleConflict;
                result.ConflictingActivities.AddRange(conflicts);
                result.ConflictMessages.Add($"Bạn đã đăng ký {conflicts.Count} hoạt động khác trong cùng thời gian.");
            }

            return result;
        }

        public async Task<List<TimeSlot>> FindAvailableTimeSlotsAsync(int maDiaDiem, DateTime startDate, DateTime endDate, TimeSpan duration)
        {
            var timeSlots = new List<TimeSlot>();
            var currentDate = startDate.Date;

            while (currentDate <= endDate.Date)
            {
                // Tạo các khoảng thời gian trong ngày (8h-22h)
                var dayStart = currentDate.AddHours(8);
                var dayEnd = currentDate.AddHours(22);

                // Lấy các hoạt động đã có trong ngày tại địa điểm này
                var existingActivities = await _context.HoatDongs
                    .Where(h => h.MaDiaDiem == maDiaDiem &&
                               h.MaTrangThai != 3 &&
                               h.ThoiGianBatDau.Date == currentDate)
                    .OrderBy(h => h.ThoiGianBatDau)
                    .ToListAsync();

                var availableSlots = FindAvailableSlotsInDay(dayStart, dayEnd, existingActivities, duration);
                timeSlots.AddRange(availableSlots);

                currentDate = currentDate.AddDays(1);
            }

            // Sắp xếp theo điểm số (thời gian gần hiện tại hơn có điểm cao hơn)
            var now = DateTime.Now;
            foreach (var slot in timeSlots)
            {
                slot.Score = CalculateTimeSlotScore(slot, now);
            }

            return timeSlots.OrderByDescending(s => s.Score).ToList();
        }

        public async Task<List<TimeSlot>> SuggestOptimalTimeSlotsAsync(HoatDong hoatDong)
        {
            var suggestions = new List<TimeSlot>();
            var startDate = DateTime.Now.Date;
            var endDate = startDate.AddDays(30); // Tìm trong 30 ngày tới

            // Tìm các khoảng thời gian trống cho địa điểm hiện tại
            var availableSlots = await FindAvailableTimeSlotsAsync(hoatDong.MaDiaDiem, startDate, endDate, 
                hoatDong.ThoiGianKetThuc - hoatDong.ThoiGianBatDau);

            // Sắp xếp theo điểm số (thời gian gần hiện tại và thời gian thuận tiện)
            var scoredSlots = availableSlots.Select(slot => new
            {
                Slot = slot,
                Score = CalculateTimeSlotScore(slot, DateTime.Now)
            }).OrderByDescending(x => x.Score).Take(5);

            suggestions.AddRange(scoredSlots.Select(x => x.Slot));

            // Nếu không tìm thấy slot cho địa điểm hiện tại, tìm các địa điểm khác
            if (!suggestions.Any())
            {
                var otherDiaDiems = await _context.DiaDiems
                    .Where(d => d.TrangThai && d.MaDiaDiem != hoatDong.MaDiaDiem)
                    .ToListAsync();

                foreach (var diaDiem in otherDiaDiems.Take(3))
                {
                    var slotsForDiaDiem = await FindAvailableTimeSlotsAsync(diaDiem.MaDiaDiem, startDate, endDate,
                        hoatDong.ThoiGianKetThuc - hoatDong.ThoiGianBatDau);

                    if (slotsForDiaDiem.Any())
                    {
                        suggestions.AddRange(slotsForDiaDiem.Take(2));
                        break;
                    }
                }
            }

            return suggestions;
        }

        public async Task<List<TimeSlot>> GetAlternativeTimeSlotsAsync(HoatDong hoatDong, int maxSuggestions = 5)
        {
            var suggestions = new List<TimeSlot>();
            var startDate = DateTime.Now.Date;
            var endDate = startDate.AddDays(14); // Tìm trong 14 ngày tới

            // Tìm các khoảng thời gian trống cho địa điểm hiện tại
            var availableSlots = await FindAvailableTimeSlotsAsync(hoatDong.MaDiaDiem, startDate, endDate, 
                hoatDong.ThoiGianKetThuc - hoatDong.ThoiGianBatDau);

            // Sắp xếp theo điểm số và lấy top suggestions
            var scoredSlots = availableSlots.Select(slot => new
            {
                Slot = slot,
                Score = CalculateTimeSlotScore(slot, DateTime.Now)
            }).OrderByDescending(x => x.Score).Take(maxSuggestions);

            suggestions.AddRange(scoredSlots.Select(x => x.Slot));

            return suggestions;
        }

        public async Task<bool> CheckDuplicateRegistrationAsync(string userId, int maHoatDong)
        {
            return await _context.DangKyThamGias
                .AnyAsync(d => d.UserId == userId && d.MaHoatDong == maHoatDong);
        }

        public async Task<List<HoatDong>> GetConflictingActivitiesAsync(HoatDong hoatDong, int? excludeHoatDongId = null)
        {
            return await _context.HoatDongs
                .Include(h => h.DiaDiem)
                .Include(h => h.TrangThaiHoatDong)
                .Where(h => h.MaDiaDiem == hoatDong.MaDiaDiem &&
                           h.MaTrangThai != 3 &&
                           h.MaHoatDong != excludeHoatDongId &&
                           (h.ThoiGianBatDau < hoatDong.ThoiGianKetThuc && 
                            h.ThoiGianKetThuc > hoatDong.ThoiGianBatDau))
                .ToListAsync();
        }

        private List<TimeSlot> FindAvailableSlotsInDay(DateTime dayStart, DateTime dayEnd, List<HoatDong> existingActivities, TimeSpan duration)
        {
            var slots = new List<TimeSlot>();
            var currentTime = dayStart;

            while (currentTime.Add(duration) <= dayEnd)
            {
                var slotEnd = currentTime.Add(duration);
                var hasConflict = false;

                foreach (var activity in existingActivities)
                {
                    if (currentTime < activity.ThoiGianKetThuc && slotEnd > activity.ThoiGianBatDau)
                    {
                        hasConflict = true;
                        break;
                    }
                }

                if (!hasConflict)
                {
                    slots.Add(new TimeSlot
                    {
                        StartTime = currentTime,
                        EndTime = slotEnd
                    });
                }

                currentTime = currentTime.AddMinutes(30); // Tăng 30 phút mỗi lần
            }

            return slots;
        }

        private double CalculateTimeSlotScore(TimeSlot slot, DateTime now)
        {
            var score = 100.0;

            // Ưu tiên thời gian gần hiện tại
            var daysFromNow = (slot.StartTime - now).TotalDays;
            if (daysFromNow <= 7) score += 20;
            else if (daysFromNow <= 14) score += 10;

            // Ưu tiên thời gian trong giờ hành chính
            var hour = slot.StartTime.Hour;
            if (hour >= 8 && hour <= 17) score += 15;
            else if (hour >= 18 && hour <= 20) score += 10;

            // Ưu tiên cuối tuần
            if (slot.StartTime.DayOfWeek == DayOfWeek.Saturday || slot.StartTime.DayOfWeek == DayOfWeek.Sunday)
                score += 10;

            return score;
        }
    }
} 