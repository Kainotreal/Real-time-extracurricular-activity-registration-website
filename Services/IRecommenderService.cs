using System.Collections.Generic;
using System.Threading.Tasks;
using QuanLyHoatDongNgoaiKhoa.Models;

namespace QuanLyHoatDongNgoaiKhoa.Services
{
    public interface IRecommenderService
    {
        Task<List<int>> GetRecommendedActivityIdsAsync(int currentActivityId, List<HoatDong> allActivities, int topK = 4);
    }
}
