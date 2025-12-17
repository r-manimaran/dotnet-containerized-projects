using UserManagementApi.Data;
using Wolverine;

namespace UserManagementApi.Features.Register;

public class RegisterUserHandler(AppDbContext dbContext)
{
    public async Task<Guid> Handle(RegisterUser command, IMessageBus bus)
    {
        var user = new User
        {
            Id = Guid.CreateVersion7(),
            Email = command.Email,
            FirstName = command.FirstName,
            LastName = command.LastName,
            CreatedAt = DateTime.UtcNow
        };
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        //return new UserRegistered(user.Id);
        await bus.PublishAsync(new UserRegistered(user.Id));
        return user.Id;
    }
}
