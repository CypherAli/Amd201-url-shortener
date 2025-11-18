using AMD201.Core.DTOs;
using AMD201.Core.Entities;
using AMD201.Core.Interfaces;
using AMD201.Infrastructure.Services;
using Moq;
using Xunit;

namespace AMD201.Tests.Services
{
    public class UrlShortenerServiceTests
    {
        private readonly Mock<IUrlRepository> _mockRepository;
        private readonly Mock<IQrCodeService> _mockQrCodeService;
        private readonly UrlShortenerService _service;

        public UrlShortenerServiceTests()
        {
            _mockRepository = new Mock<IUrlRepository>();
            _mockQrCodeService = new Mock<IQrCodeService>();
            _mockQrCodeService.Setup(q => q.GenerateQrCode(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new byte[] { 0x00, 0x01, 0x02 });
            _service = new UrlShortenerService(_mockRepository.Object, _mockQrCodeService.Object);
        }

        [Fact]
        public async Task ShortenUrlAsync_ValidUrl_ReturnsShortCode()
        {
            // Arrange
            var request = new ShortenUrlRequest
            {
                OriginalUrl = "https://www.example.com/very/long/url/path"
            };
            _mockRepository.Setup(r => r.ExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<ShortenedUrl>())).ReturnsAsync((ShortenedUrl url) => url);

            // Act
            var result = await _service.ShortenUrlAsync(request, null, "https://localhost");

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.ShortCode);
            Assert.Equal(request.OriginalUrl, result.OriginalUrl);
            Assert.Contains("https://localhost/", result.ShortUrl);
        }

        [Fact]
        public async Task ShortenUrlAsync_InvalidUrl_ThrowsArgumentException()
        {
            // Arrange
            var request = new ShortenUrlRequest
            {
                OriginalUrl = "not-a-valid-url"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.ShortenUrlAsync(request, null, "https://localhost"));
        }

        [Fact]
        public async Task ShortenUrlAsync_CustomCodeWithoutAuth_ThrowsUnauthorizedException()
        {
            // Arrange
            var request = new ShortenUrlRequest
            {
                OriginalUrl = "https://www.example.com",
                CustomCode = "my-custom-code"
            };

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.ShortenUrlAsync(request, null, "https://localhost"));
        }

        [Fact]
        public async Task ShortenUrlAsync_CustomCodeWithAuth_CreatesCustomUrl()
        {
            // Arrange
            var request = new ShortenUrlRequest
            {
                OriginalUrl = "https://www.example.com",
                CustomCode = "my-link"
            };
            _mockRepository.Setup(r => r.ExistsAsync("my-link")).ReturnsAsync(false);
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<ShortenedUrl>())).ReturnsAsync((ShortenedUrl url) => url);

            // Act
            var result = await _service.ShortenUrlAsync(request, "user123", "https://localhost");

            // Assert
            Assert.Equal("my-link", result.ShortCode);
            Assert.Contains("/my-link", result.ShortUrl);
        }

        [Fact]
        public async Task ShortenUrlAsync_ExistingCustomCode_ThrowsInvalidOperationException()
        {
            // Arrange
            var request = new ShortenUrlRequest
            {
                OriginalUrl = "https://www.example.com",
                CustomCode = "taken-code"
            };
            _mockRepository.Setup(r => r.ExistsAsync("taken-code")).ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.ShortenUrlAsync(request, "user123", "https://localhost"));
        }

        [Fact]
        public async Task GetOriginalUrlAsync_ExistingCode_ReturnsUrl()
        {
            // Arrange
            var shortCode = "abc123";
            var expectedUrl = "https://www.example.com";
            var shortenedUrl = new ShortenedUrl
            {
                ShortCode = shortCode,
                OriginalUrl = expectedUrl,
                IsActive = true
            };
            _mockRepository.Setup(r => r.GetByShortCodeAsync(shortCode)).ReturnsAsync(shortenedUrl);

            // Act
            var result = await _service.GetOriginalUrlAsync(shortCode);

            // Assert
            Assert.Equal(expectedUrl, result);
        }

        [Fact]
        public async Task GetOriginalUrlAsync_NonExistingCode_ReturnsNull()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetByShortCodeAsync(It.IsAny<string>())).ReturnsAsync((ShortenedUrl?)null);

            // Act
            var result = await _service.GetOriginalUrlAsync("nonexistent");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetOriginalUrlAsync_ExpiredUrl_ReturnsNull()
        {
            // Arrange
            var shortCode = "expired";
            var shortenedUrl = new ShortenedUrl
            {
                ShortCode = shortCode,
                OriginalUrl = "https://www.example.com",
                IsActive = true,
                ExpiresAt = DateTime.UtcNow.AddDays(-1) // Expired yesterday
            };
            _mockRepository.Setup(r => r.GetByShortCodeAsync(shortCode)).ReturnsAsync(shortenedUrl);

            // Act
            var result = await _service.GetOriginalUrlAsync(shortCode);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task IsShortCodeAvailableAsync_NewCode_ReturnsTrue()
        {
            // Arrange
            _mockRepository.Setup(r => r.ExistsAsync("newcode")).ReturnsAsync(false);

            // Act
            var result = await _service.IsShortCodeAvailableAsync("newcode");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsShortCodeAvailableAsync_ExistingCode_ReturnsFalse()
        {
            // Arrange
            _mockRepository.Setup(r => r.ExistsAsync("existing")).ReturnsAsync(true);

            // Act
            var result = await _service.IsShortCodeAvailableAsync("existing");

            // Assert
            Assert.False(result);
        }
    }
}
