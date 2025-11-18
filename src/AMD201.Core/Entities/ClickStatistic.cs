using System;

namespace AMD201.Core.Entities
{
    public class ClickStatistic
    {
        public int Id { get; set; }
        public int ShortenedUrlId { get; set; }
        public DateTime ClickedAt { get; set; } = DateTime.UtcNow;
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? Referrer { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }

        // Navigation property
        public virtual ShortenedUrl ShortenedUrl { get; set; } = null!;
    }
}
