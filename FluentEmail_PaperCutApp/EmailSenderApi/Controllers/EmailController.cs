using FluentEmail.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace EmailSenderApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmailController : ControllerBase
{
    private readonly IFluentEmail _fluentEmail;
    private readonly ILogger<EmailController> _logger;

    public EmailController(IFluentEmail fluentEmail, ILogger<EmailController> logger)
    {
        _fluentEmail = fluentEmail;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Send(EmailSendRequest request)
    {
        _logger.LogInformation("Sending Email using FluentEmail");
        var response = await _fluentEmail
                                .To(request.ToAddress)
                                .Subject(request.Subject)
                                .Body(request.Message, isHtml: true)
                                .SendAsync();

        return (response.Successful) ? Ok(response) : BadRequest();
    }
}


public record EmailSendRequest(string ToAddress, string Subject, string Message);
