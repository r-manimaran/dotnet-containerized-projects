using UserManagementApi.Data;

namespace UserManagementApi.Features.Register;

public class RegisterUserHandler(AppDbContext dbContext)
{
    public async Task<UserRegistered> Handle(RegisterUser command)
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

        return new UserRegistered(user.Id);
    }
}
