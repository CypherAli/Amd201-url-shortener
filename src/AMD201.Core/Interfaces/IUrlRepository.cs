using AMD201.Core.Entities;

namespace AMD201.Core.Interfaces
{
    public interface IUrlRepository
    {
        Task<ShortenedUrl?> GetByShortCodeAsync(string shortCode);
        Task<ShortenedUrl?> GetByIdAsync(int id);
        Task<ShortenedUrl> AddAsync(ShortenedUrl url);
        Task<bool> UpdateAsync(ShortenedUrl url);
        Task<bool> DeleteAsync(ShortenedUrl url);
        Task<bool> ExistsAsync(string shortCode);
        Task<List<ShortenedUrl>> GetByUserIdAsync(string userId, int skip, int take);
        Task<int> GetCountByUserIdAsync(string userId);
        Task<bool> AddClickStatisticAsync(ClickStatistic statistic);
        Task<List<ClickStatistic>> GetClickStatisticsByUrlIdAsync(int urlId, int limit);
    }
}
