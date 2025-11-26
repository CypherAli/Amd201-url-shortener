using AMD201.Core.DTOs;
using AMD201.Core.Entities;
using AMD201.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AMD201.API.Controllers
{
    [ApiController]
    [Route("api///[controller]")]
    public class UrlController : ControllerBase
    {
        private readonly IUrlShortenerService _urlService;
        private readonly IQrCodeService _qrCodeService;
        private readonly ILogger<UrlController> _logger;

        public UrlController(IUrlShortenerService urlService, IQrCodeService qrCodeService, ILogger<UrlController> logger)
        {
            _urlService = urlService;
            _qrCodeService = qrCodeService;
            _logger = logger;
        }

        /// <summary>
        /// Shorten a URL (supports both authenticated and anonymous users)
        /// </summary>
        [HttpPost("shorten")]
        [ProducesResponseType(typeof(ShortenUrlResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ShortenUrl([FromBody] ShortenUrlRequest request)
        {
            try
            {
                var userId = HttpContext.Items["UserId"]?.ToString();
                var baseUrl = $"{Request.Scheme}://{Request.Host.Value}";

                var result = await _urlService.ShortenUrlAsync(request, userId, baseUrl);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error shortening URL");
                return StatusCode(500, new { error = "An error occurred while shortening the URL" });
            }
        }

        /// <summary>
        /// Get user's URL history (authenticated users only)
        /// </summary>
        [HttpGet("history")]
        [ProducesResponseType(typeof(UserHistoryResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetHistory([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var userId = HttpContext.Items["UserId"]?.ToString();
            
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { error = "Authentication required" });
            }

            try
            {
                var result = await _urlService.GetUserHistoryAsync(userId, page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving history for user {UserId}", userId);
                return StatusCode(500, new { error = "An error occurred while retrieving history" });
            }
        }

        /// <summary>
        /// Get statistics for a specific shortened URL
        /// </summary>
        [HttpGet("stats/{shortCode}")]
        [ProducesResponseType(typeof(UrlStatisticsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStatistics(string shortCode)
        {
            try
            {
                var userId = HttpContext.Items["UserId"]?.ToString();
                var result = await _urlService.GetUrlStatisticsAsync(shortCode, userId);

                if (result == null)
                {
                    return NotFound(new { error = "Short URL not found" });
                }

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving statistics for {ShortCode}", shortCode);
                return StatusCode(500, new { error = "An error occurred while retrieving statistics" });
            }
        }

        /// <summary>
        /// Check if a custom short code is available
        /// </summary>
        [HttpGet("check/{shortCode}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckAvailability(string shortCode)
        {
            var isAvailable = await _urlService.IsShortCodeAvailableAsync(shortCode);
            return Ok(new { shortCode, available = isAvailable });
        }

        /// <summary>
        /// Delete a shortened URL (authenticated users only, can only delete own URLs)
        /// </summary>
        [HttpDelete("{shortCode}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUrl(string shortCode)
        {
            var userId = HttpContext.Items["UserId"]?.ToString();
            
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { error = "Authentication required" });
            }

            try
            {
                var result = await _urlService.DeleteUrlAsync(shortCode, userId);
                
                if (!result)
                {
                    return NotFound(new { error = "URL not found or you don't have permission to delete it" });
                }

                return Ok(new { message = "URL deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting URL {ShortCode} for user {UserId}", shortCode, userId);
                return StatusCode(500, new { error = "An error occurred while deleting the URL" });
            }
        }

        /// <summary>
        /// Update a shortened URL (change custom code or original URL)
        /// </summary>
        [HttpPut("{shortCode}")]
        [ProducesResponseType(typeof(ShortenUrlResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUrl(string shortCode, [FromBody] UpdateUrlRequest request)
        {
            var userId = HttpContext.Items["UserId"]?.ToString();
            
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { error = "Authentication required" });
            }

            try
            {
                var baseUrl = $"{Request.Scheme}://{Request.Host.Value}";
                var result = await _urlService.UpdateUrlAsync(shortCode, request, userId, baseUrl);
                
                if (result == null)
                {
                    return NotFound(new { error = "URL not found or you don't have permission to update it" });
                }

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating URL {ShortCode} for user {UserId}", shortCode, userId);
                return StatusCode(500, new { error = "An error occurred while updating the URL" });
            }
        }

        /// <summary>
        /// Get QR code for a shortened URL as PNG image - QR contains the ORIGINAL URL
        /// </summary>
        [HttpGet("qr/{shortCode}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetQrCode(string shortCode, [FromQuery] int size = 10)
        {
            try
            {
                var originalUrl = await _urlService.GetOriginalUrlAsync(shortCode);
                
                if (originalUrl == null)
                {
                    return NotFound(new { error = "Short URL not found or expired" });
                }

                // QR code chứa LINK GỐC, không phải link rút gọn
                // Khi scan QR code = điện thoại → truy cập trực tiếp vào trang gốc
                var qrCodeBytes = _qrCodeService.GenerateQrCode(originalUrl, size);
                
                return File(qrCodeBytes, "image/png");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating QR code for {ShortCode}", shortCode);
                return StatusCode(500, new { error = "An error occurred while generating QR code" });
            }
        }
    }
}
