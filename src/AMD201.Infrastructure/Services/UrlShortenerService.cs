using AMD201.Core.DTOs;
using AMD201.Core.Entities;
using AMD201.Core.Interfaces;
using System.Text;

namespace AMD201.Infrastructure.Services
{
    public class UrlShortenerService : IUrlShortenerService
    {
        private readonly IUrlRepository _urlRepository;
        private readonly IQrCodeService _qrCodeService;
        private const string Characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private const int ShortCodeLength = 6;

        public UrlShortenerService(IUrlRepository urlRepository, IQrCodeService qrCodeService)
        {
            _urlRepository = urlRepository;
            _qrCodeService = qrCodeService;
        }

        public async Task<ShortenUrlResponse> ShortenUrlAsync(ShortenUrlRequest request, string? userId, string baseUrl)
        {
            // Validate URL
            if (!IsValidUrl(request.OriginalUrl))
            {
                throw new ArgumentException("Invalid URL format");
            }

            string shortCode;

            if (!string.IsNullOrEmpty(request.CustomCode))
            {
                // Custom code for authenticated users
                if (string.IsNullOrEmpty(userId))
                {
                    throw new UnauthorizedAccessException("Custom codes are only available for authenticated users");
                }

                if (!IsValidCustomCode(request.CustomCode))
                {
                    throw new ArgumentException("Custom code must be 3-20 characters, alphanumeric and hyphens only");
                }

                if (await _urlRepository.ExistsAsync(request.CustomCode))
                {
                    throw new InvalidOperationException($"Custom code '{request.CustomCode}' is already taken");
                }

                shortCode = request.CustomCode;
            }
            else
            {
                // Generate random code
                shortCode = await GenerateUniqueShortCodeAsync();
            }

            var shortenedUrl = new ShortenedUrl
            {
                ShortCode = shortCode,
                OriginalUrl = request.OriginalUrl,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                IsCustom = !string.IsNullOrEmpty(request.CustomCode),
                ExpiresAt = request.ExpiresInDays.HasValue 
                    ? DateTime.UtcNow.AddDays(request.ExpiresInDays.Value) 
                    : null
            };

            await _urlRepository.AddAsync(shortenedUrl);

            var shortUrl = $"{baseUrl}/{shortCode}";
            return new ShortenUrlResponse
            {
                ShortCode = shortCode,
                ShortUrl = shortUrl,
                OriginalUrl = request.OriginalUrl,
                CreatedAt = shortenedUrl.CreatedAt,
                ExpiresAt = shortenedUrl.ExpiresAt,
                QrCodeUrl = $"{baseUrl}/api/url/qr/{shortCode}"
            };
        }

        public async Task<string?> GetOriginalUrlAsync(string shortCode)
        {
            var url = await _urlRepository.GetByShortCodeAsync(shortCode);
            
            if (url == null) return null;
            
            // Check if expired
            if (url.ExpiresAt.HasValue && url.ExpiresAt.Value < DateTime.UtcNow)
            {
                return null;
            }

            return url.OriginalUrl;
        }

        public async Task<bool> IncrementClickCountAsync(string shortCode, ClickStatistic clickData)
        {
            var url = await _urlRepository.GetByShortCodeAsync(shortCode);
            if (url == null) return false;

            url.ClickCount++;
            clickData.ShortenedUrlId = url.Id;

            await _urlRepository.UpdateAsync(url);
            await _urlRepository.AddClickStatisticAsync(clickData);

            return true;
        }

        public async Task<UrlStatisticsResponse?> GetUrlStatisticsAsync(string shortCode, string? userId)
        {
            var url = await _urlRepository.GetByShortCodeAsync(shortCode);
            if (url == null) return null;

            // Only authenticated users can see stats of their own URLs
            if (url.UserId != null && url.UserId != userId)
            {
                throw new UnauthorizedAccessException("You don't have permission to view these statistics");
            }

            var recentClicks = await _urlRepository.GetClickStatisticsByUrlIdAsync(url.Id, 100);

            return new UrlStatisticsResponse
            {
                ShortCode = url.ShortCode,
                OriginalUrl = url.OriginalUrl,
                TotalClicks = url.ClickCount,
                CreatedAt = url.CreatedAt,
                RecentClicks = recentClicks.Select(c => new ClickDetailDto
                {
                    ClickedAt = c.ClickedAt,
                    Country = c.Country,
                    City = c.City,
                    Referrer = c.Referrer
                }).ToList()
            };
        }

        public async Task<UserHistoryResponse> GetUserHistoryAsync(string userId, int page = 1, int pageSize = 20)
        {
            var skip = (page - 1) * pageSize;
            var urls = await _urlRepository.GetByUserIdAsync(userId, skip, pageSize);
            var totalCount = await _urlRepository.GetCountByUserIdAsync(userId);

            return new UserHistoryResponse
            {
                Urls = urls.Select(u => new UserUrlDto
                {
                    ShortCode = u.ShortCode,
                    ShortUrl = $"/{u.ShortCode}",
                    OriginalUrl = u.OriginalUrl,
                    ClickCount = u.ClickCount,
                    CreatedAt = u.CreatedAt,
                    IsCustom = u.IsCustom,
                    QrCodeUrl = $"/api/url/qr/{u.ShortCode}"
                }).ToList(),
                TotalUrls = totalCount,
                TotalClicks = urls.Sum(u => u.ClickCount)
            };
        }

        public async Task<bool> IsShortCodeAvailableAsync(string shortCode)
        {
            return !await _urlRepository.ExistsAsync(shortCode);
        }

        public async Task<bool> DeleteUrlAsync(string shortCode, string userId)
        {
            var url = await _urlRepository.GetByShortCodeAsync(shortCode);
            
            if (url == null || url.UserId != userId)
            {
                return false;
            }

            await _urlRepository.DeleteAsync(url);
            return true;
        }

        public async Task<ShortenUrlResponse?> UpdateUrlAsync(string shortCode, UpdateUrlRequest request, string userId, string baseUrl)
        {
            var url = await _urlRepository.GetByShortCodeAsync(shortCode);
            
            if (url == null || url.UserId != userId)
            {
                return null;
            }

            // Update original URL if provided
            if (!string.IsNullOrEmpty(request.OriginalUrl))
            {
                if (!IsValidUrl(request.OriginalUrl))
                {
                    throw new ArgumentException("Invalid URL format");
                }
                url.OriginalUrl = request.OriginalUrl;
            }

            // Handle short code change
            string newShortCode = shortCode;
            
            if (request.GenerateRandom)
            {
                // Generate new random code
                newShortCode = await GenerateUniqueShortCodeAsync();
                url.ShortCode = newShortCode;
                url.IsCustom = false;
            }
            else if (!string.IsNullOrEmpty(request.NewCustomCode) && request.NewCustomCode != shortCode)
            {
                // Change to new custom code
                if (!IsValidCustomCode(request.NewCustomCode))
                {
                    throw new ArgumentException("Custom code must be 3-20 characters, alphanumeric and hyphens only");
                }

                if (await _urlRepository.ExistsAsync(request.NewCustomCode))
                {
                    throw new InvalidOperationException($"Custom code '{request.NewCustomCode}' is already taken");
                }

                newShortCode = request.NewCustomCode;
                url.ShortCode = newShortCode;
                url.IsCustom = true;
            }

            await _urlRepository.UpdateAsync(url);

            var shortUrl = $"{baseUrl}/{newShortCode}";
            return new ShortenUrlResponse
            {
                ShortCode = newShortCode,
                ShortUrl = shortUrl,
                OriginalUrl = url.OriginalUrl,
                CreatedAt = url.CreatedAt,
                ExpiresAt = url.ExpiresAt,
                QrCodeUrl = $"{baseUrl}/api/url/qr/{newShortCode}"
            };
        }

        private async Task<string> GenerateUniqueShortCodeAsync()
        {
            string shortCode;
            int attempts = 0;
            const int maxAttempts = 10;

            do
            {
                shortCode = GenerateRandomCode();
                attempts++;

                if (attempts >= maxAttempts)
                {
                    // If collision happens too many times, increase length
                    shortCode = GenerateRandomCode(ShortCodeLength + 1);
                }

            } while (await _urlRepository.ExistsAsync(shortCode));

            return shortCode;
        }

        private string GenerateRandomCode(int length = ShortCodeLength)
        {
            var random = new Random();
            var code = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                code.Append(Characters[random.Next(Characters.Length)]);
            }

            return code.ToString();
        }

        private bool IsValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        private bool IsValidCustomCode(string code)
        {
            if (string.IsNullOrEmpty(code) || code.Length < 3 || code.Length > 20)
                return false;

            return code.All(c => char.IsLetterOrDigit(c) || c == '-');
        }
    }
}
