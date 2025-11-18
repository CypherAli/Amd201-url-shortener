namespace AMD201.Core.Interfaces
{
    public interface IQrCodeService
    {
        /// <summary>
        /// Generate QR code as PNG byte array
        /// </summary>
        byte[] GenerateQrCode(string text, int pixelsPerModule = 10);
        
        /// <summary>
        /// Generate QR code as Base64 string
        /// </summary>
        string GenerateQrCodeBase64(string text, int pixelsPerModule = 10);
    }
}
