using AMD201.Core.DTOs;
using AMD201.Core.Entities;

namespace AMD201.Core.Interfaces
{
    public interface IUrlShortenerService
    {
        Task<ShortenUrlResponse> ShortenUrlAsync(ShortenUrlRequest request, string? userId, string baseUrl);
        Task<string?> GetOriginalUrlAsync(string shortCode);
        Task<bool> IncrementClickCountAsync(string shortCode, ClickStatistic clickData);
        Task<UrlStatisticsResponse?> GetUrlStatisticsAsync(string shortCode, string? userId);
        Task<UserHistoryResponse> GetUserHistoryAsync(string userId, int page = 1, int pageSize = 20);
        Task<bool> IsShortCodeAvailableAsync(string shortCode);
        Task<bool> DeleteUrlAsync(string shortCode, string userId);
        Task<ShortenUrlResponse?> UpdateUrlAsync(string shortCode, UpdateUrlRequest request, string userId, string baseUrl);
    }
}
