using QRCoder;
using System.Drawing.Imaging;
using System.Drawing;


namespace BusinesscardsApi.Services;

public interface IQRCodeService
{
    byte[] GenerateQRCode(string content);
    byte[] GenerateQRCodeWithLogo(string content);
}

public class QRCodeService : IQRCodeService
{
    public byte[] GenerateQRCode(string content)
    {
        using QRCodeGenerator qrGenerator = new QRCodeGenerator();

        using QRCodeData qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);

        using QRCode qrCode = new QRCode(qrCodeData);

        using Bitmap qrCodeImage = qrCode.GetGraphic(20);

        using MemoryStream stream = new MemoryStream();

        qrCodeImage.Save(stream, ImageFormat.Png);

        return stream.ToArray();

    }

    public byte[] GenerateQRCodeWithLogo(string content)
    {
        using QRCodeGenerator qrGenerator = new QRCodeGenerator();

        using QRCodeData qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);

        using QRCode qrCode = new QRCode(qrCodeData);

        using Bitmap qrCodeImage = qrCode.GetGraphic(20);

        string logoPath = "logo.png";

        using Bitmap logoBitmp = new Bitmap(logoPath);

        int logoSize = qrCodeImage.Width / 5;
        int logoX = (qrCodeImage.Width - logoSize) / 2;
        int logoY = (qrCodeImage.Height - logoSize) / 2;

        using Graphics graphics = Graphics.FromImage(qrCodeImage);

        graphics.DrawImage(logoBitmp, new Rectangle(logoX, logoY, logoSize, logoSize));

        using MemoryStream stream = new MemoryStream();

        qrCodeImage.Save(stream, ImageFormat.Png);

        return stream.ToArray();
    }
}



