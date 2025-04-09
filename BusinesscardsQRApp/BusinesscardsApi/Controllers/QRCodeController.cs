using BusinesscardsApi.Models;
using BusinesscardsApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;

namespace BusinesscardsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QRCodeController : ControllerBase
    {
        private readonly IQRCodeService _qrCodeService;

        public QRCodeController(IQRCodeService qrCodeService)
        {
            _qrCodeService = qrCodeService;
        }
        [HttpPost("generate")]
        public IActionResult GenerateQRCode([FromBody] BusinessCard businessCard)
        {
            // Format as vCard for better mobile compatibility
            string vCard = $"BEGIN:VCARD\n" +
                          $"VERSION:3.0\n" +
                          $"N:{businessCard.FullName}\n" +
                          $"ORG:{businessCard.CompanyName}\n" +
                          $"TITLE:{businessCard.Title}\n" +
                          $"TEL:{businessCard.Phone}\n" +
                          $"EMAIL:{businessCard.Email}\n" +
                          $"URL:{businessCard.Website}\n" +
                          $"ADR:{businessCard.Address}\n" +
                          $"END:VCARD";
            var qrCode = _qrCodeService.GenerateQRCode(vCard);
            return File(qrCode, "image/png");

        }

        [HttpPost("generateWithLogo")]
        public IActionResult GenerateQRWithLogo([FromBody] BusinessCard businessCard)
        {
            // Format as vCard for better mobile compatibility
            string vCard = $"BEGIN:VCARD\n" +
                          $"VERSION:3.0\n" +
                          $"FN:{businessCard.FullName}\n" +
                          $"ORG:{businessCard.CompanyName}\n" +
                          $"TITLE:{businessCard.Title}\n" +
                          $"TEL:{businessCard.Phone}\n" +
                          $"EMAIL:{businessCard.Email}\n" +
                          $"URL:{businessCard.Website}\n" +
                          $"ADR:{businessCard.Address}\n" +
                          $"END:VCARD";
            var qrCode = _qrCodeService.GenerateQRCodeWithLogo(vCard);
            return File(qrCode, "image/png");
        }
    }
}
