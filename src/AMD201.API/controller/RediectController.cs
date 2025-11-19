using AMD201.Core.Entities;
using AMD201.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AMD201.API.Controllers
{
    [ApiController]
    public class RedirectController : ControllerBase
    {
        private readonly IUrlShortenerService _urlService;
        private readonly ILogger<RedirectController> _logger;

        public RedirectController(IUrlShortenerService urlService, ILogger<RedirectController> logger)
        {
            _urlService = urlService;
            _logger = logger;
        }

        /// <summary>
        /// Redirect to original URL from short code
        /// </summary>
        [HttpGet("/{shortCode}")]
        public async Task<IActionResult> RedirectToUrl(string shortCode)
        {
            try
            {
                var originalUrl = await _urlService.GetOriginalUrlAsync(shortCode);

                if (originalUrl == null)
                {
                    return NotFound("Short URL not found or expired");
                }

                // Track click statistics
                var clickData = new ClickStatistic
                {
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    UserAgent = Request.Headers["User-Agent"].ToString(),
                    Referrer = Request.Headers["Referer"].ToString(),
                    ClickedAt = DateTime.UtcNow
                };

                // Fire and forget - don't wait for stats to be saved
                _ = _urlService.IncrementClickCountAsync(shortCode, clickData);

                // Redirect with 302 Found (temporary redirect)
                return Redirect(originalUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error redirecting {ShortCode}", shortCode);
                return StatusCode(500, "An error occurred");
            }
        }
    }
}
