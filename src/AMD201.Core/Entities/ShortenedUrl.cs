using System;

namespace AMD201.Core.Entities
{
    public class ShortenedUrl
    {
        public int Id { get; set; }
        public string ShortCode { get; set; } = string.Empty;
        public string OriginalUrl { get; set; } = string.Empty
        public string? UserId { get; set; } // Null for anonymous users
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int ClickCount { get; set; } = 0;
        public bool IsCustom { get; set; } = false;
        public DateTime? ExpiresAt { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation property
        public virtual ICollection<ClickStatistic> ClickStatistics { get; set; } = new List<ClickStatistic>();
    }
}
