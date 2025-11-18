using AMD201.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace AMD201.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ShortenedUrl> ShortenedUrls { get; set; }
        public DbSet<ClickStatistic> ClickStatistics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ShortenedUrl>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.ShortCode).IsUnique();
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.CreatedAt);

                entity.Property(e => e.ShortCode)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.OriginalUrl)
                    .IsRequired()
                    .HasMaxLength(2048);

                entity.Property(e => e.UserId)
                    .HasMaxLength(100);

                entity.HasMany(e => e.ClickStatistics)
                    .WithOne(e => e.ShortenedUrl)
                    .HasForeignKey(e => e.ShortenedUrlId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ClickStatistic>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.ShortenedUrlId);
                entity.HasIndex(e => e.ClickedAt);

                entity.Property(e => e.IpAddress)
                    .HasMaxLength(45); // IPv6 max length

                entity.Property(e => e.UserAgent)
                    .HasMaxLength(512);

                entity.Property(e => e.Referrer)
                    .HasMaxLength(512);

                entity.Property(e => e.Country)
                    .HasMaxLength(100);

                entity.Property(e => e.City)
                    .HasMaxLength(100);
            });
        }
    }
}
