namespace AMD201.Core.DTOs
{
    public class ShortenUrlRequest
    {
        public string OriginalUrl { get; set; } = string.Empty;
        public string? CustomCode { get; set; }
        public int? ExpiresInDays { get; set; }
    }

    public class UpdateUrlRequest
    {
        public string? OriginalUrl { get; set; }
        public string? NewCustomCode { get; set; }
        public bool GenerateRandom { get; set; } = false;
    }

    public class ShortenUrlResponse
    {
        public string ShortCode { get; set; } = string.Empty;
        public string ShortUrl { get; set; } = string.Empty;
        public string OriginalUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }

    public class UrlStatisticsResponse
    {
        public string ShortCode { get; set; } = string.Empty;
        public string OriginalUrl { get; set; } = string.Empty;
        public int TotalClicks { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ClickDetailDto> RecentClicks { get; set; } = new();
    }

    public class ClickDetailDto
    {
        public DateTime ClickedAt { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? Referrer { get; set; }
    }

    public class UserHistoryResponse
    {
        public List<UserUrlDto> Urls { get; set; } = new();
        public int TotalUrls { get; set; }
        public int TotalClicks { get; set; }
    }

    public class UserUrlDto
    {
        public string ShortCode { get; set; } = string.Empty;
        public string ShortUrl { get; set; } = string.Empty;
        public string OriginalUrl { get; set; } = string.Empty;
        public int ClickCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsCustom { get; set; }
    }
}
