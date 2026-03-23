using QuanLyHoatDongNgoaiKhoa.Models;

namespace QuanLyHoatDongNgoaiKhoa.Services
{
    public interface IScheduleOptimizationService
    {
        /// <summary>
        /// Kiểm tra xung đột thời gian và địa điểm
        /// </summary>
        Task<ScheduleConflictResult> CheckScheduleConflictAsync(HoatDong hoatDong, int? excludeHoatDongId = null);
        
        /// <summary>
        /// Kiểm tra xung đột cho người dùng cụ thể
        /// </summary>
        Task<ScheduleConflictResult> CheckUserScheduleConflictAsync(string userId, HoatDong hoatDong, int? excludeHoatDongId = null);
        
        /// <summary>
        /// Tìm kiếm khoảng thời gian trống
        /// </summary>
        Task<List<TimeSlot>> FindAvailableTimeSlotsAsync(int maDiaDiem, DateTime startDate, DateTime endDate, TimeSpan duration);
        
        /// <summary>
        /// Đề xuất thời gian tối ưu
        /// </summary>
        Task<List<TimeSlot>> SuggestOptimalTimeSlotsAsync(HoatDong hoatDong);
        
        /// <summary>
        /// Kiểm tra trùng lặp đăng ký
        /// </summary>
        Task<bool> CheckDuplicateRegistrationAsync(string userId, int maHoatDong);
        
        /// <summary>
        /// Lấy danh sách hoạt động xung đột
        /// </summary>
        Task<List<HoatDong>> GetConflictingActivitiesAsync(HoatDong hoatDong, int? excludeHoatDongId = null);
        
        /// <summary>
        /// Lấy danh sách thời gian thay thế khi có xung đột
        /// </summary>
        Task<List<TimeSlot>> GetAlternativeTimeSlotsAsync(HoatDong hoatDong, int maxSuggestions = 5);
    }

    public class ScheduleConflictResult
    {
        public bool HasConflict { get; set; }
        public List<HoatDong> ConflictingActivities { get; set; } = new();
        public List<string> ConflictMessages { get; set; } = new();
        public ConflictType ConflictType { get; set; }
    }

    public enum ConflictType
    {
        None,
        TimeOverlap,
        LocationConflict,
        UserScheduleConflict,
        DuplicateRegistration,
        ContentDuplicate,
        UserLimitExceeded
    }

    public class TimeSlot
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int DiaDiemId { get; set; }
        public string DiaDiemName { get; set; } = string.Empty;
        public double Score { get; set; } // Điểm đánh giá mức độ phù hợp
    }
} 