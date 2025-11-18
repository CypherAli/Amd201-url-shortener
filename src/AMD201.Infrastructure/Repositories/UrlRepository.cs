using AMD201.Core.Entities;
using AMD201.Core.Interfaces;
using AMD201.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AMD201.Infrastructure.Repositories
{
    public class UrlRepository : IUrlRepository
    {
        private readonly ApplicationDbContext _context;

        public UrlRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ShortenedUrl?> GetByShortCodeAsync(string shortCode)
        {
            return await _context.ShortenedUrls
                .Include(u => u.ClickStatistics)
                .FirstOrDefaultAsync(u => u.ShortCode == shortCode && u.IsActive);
        }

        public async Task<ShortenedUrl?> GetByIdAsync(int id)
        {
            return await _context.ShortenedUrls
                .Include(u => u.ClickStatistics)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<ShortenedUrl> AddAsync(ShortenedUrl url)
        {
            await _context.ShortenedUrls.AddAsync(url);
            await _context.SaveChangesAsync();
            return url;
        }

        public async Task<bool> UpdateAsync(ShortenedUrl url)
        {
            _context.ShortenedUrls.Update(url);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(ShortenedUrl url)
        {
            _context.ShortenedUrls.Remove(url);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ExistsAsync(string shortCode)
        {
            return await _context.ShortenedUrls.AnyAsync(u => u.ShortCode == shortCode);
        }

        public async Task<List<ShortenedUrl>> GetByUserIdAsync(string userId, int skip, int take)
        {
            return await _context.ShortenedUrls
                .Where(u => u.UserId == userId)
                .OrderByDescending(u => u.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> GetCountByUserIdAsync(string userId)
        {
            return await _context.ShortenedUrls
                .CountAsync(u => u.UserId == userId);
        }

        public async Task<bool> AddClickStatisticAsync(ClickStatistic statistic)
        {
            await _context.ClickStatistics.AddAsync(statistic);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<ClickStatistic>> GetClickStatisticsByUrlIdAsync(int urlId, int limit)
        {
            return await _context.ClickStatistics
                .Where(c => c.ShortenedUrlId == urlId)
                .OrderByDescending(c => c.ClickedAt)
                .Take(limit)
                .ToListAsync();
        }
    }
}
