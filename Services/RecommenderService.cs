using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using QuanLyHoatDongNgoaiKhoa.Models;

namespace QuanLyHoatDongNgoaiKhoa.Services
{
    public class RecommenderService : IRecommenderService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<RecommenderService> _logger;

        public RecommenderService(HttpClient httpClient, ILogger<RecommenderService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<int>> GetRecommendedActivityIdsAsync(int currentActivityId, List<HoatDong> allActivities, int topK = 4)
        {
            try
            {
                var requestObj = new
                {
                    target_id = currentActivityId,
                    activities = allActivities.Select(a => new
                    {
                        id = a.MaHoatDong,
                        text = $"{a.TieuDe} {a.MoTa}"
                    }).ToList(),
                    top_k = topK
                };

                var jsonContent = new StringContent(JsonSerializer.Serialize(requestObj), Encoding.UTF8, "application/json");

                // Giả định Python API chạy ở locahost:8000
                var response = await _httpClient.PostAsync("http://127.0.0.1:8000/recommend", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(responseString);
                    var recommendedIds = doc.RootElement.GetProperty("recommended_ids")
                                                        .EnumerateArray()
                                                        .Select(x => x.GetInt32())
                                                        .ToList();
                    return recommendedIds;
                }
                else
                {
                    _logger.LogWarning($"Python API call failed with status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error calling python recommender API: {ex.Message}");
            }

            return new List<int>();
        }
    }
}
