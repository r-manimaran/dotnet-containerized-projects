namespace UserManagementApi.Features.Register;

public class UserRegisteredHandler(ILogger<UserRegisteredHandler> logger)
{
    private readonly ILogger<UserRegisteredHandler> _logger = logger;

    public async Task Handle(UserRegistered @event)
    {
        _logger.LogInformation("Sending welcome email to user:{UserId}", @event.Id);

        await Task.Delay(1000);
    }
}
