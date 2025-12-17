namespace UserManagementApi.Features.Register;

public static class UserRegisteredHandler
{
    public static async Task Handle(UserRegistered @event)
    {
        await Task.Delay(1000);
    }
}
