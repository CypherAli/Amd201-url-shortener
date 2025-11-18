using AMD201.Core.Interfaces;
using QRCoder;

namespace AMD201.Infrastructure.Services
{
    public class QrCodeService : IQrCodeService
    {
        public byte[] GenerateQrCode(string text, int pixelsPerModule = 10)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            
            return qrCode.GetGraphic(pixelsPerModule);
        }

        public string GenerateQrCodeBase64(string text, int pixelsPerModule = 10)
        {
            var qrCodeBytes = GenerateQrCode(text, pixelsPerModule);
            return Convert.ToBase64String(qrCodeBytes);
        }
    }
}
