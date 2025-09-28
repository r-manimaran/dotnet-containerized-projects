namespace Aspire.AI.SQLServer.Models;

public class User
{
    public int Id { get; set; }
    public UserProfile Profile { get; set; } = null!;
}

public class UserProfile
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Age { get; set; }
    public List<string> Interests { get; set; } = new();
    public UserProfileDetails Details { get; set; } = new();
}
// Profile is a JSON column containing user profile information.
// Example: {"FirstName": "John", "LastName": "Doe", "Email": "email@gmail.com","Age":30,"interests":["tech","travel"]}

// Create some inner properties for Profile
public class UserProfileDetails
{
    public Address Address { get; set; } = new();
    public string Phone { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
}
public class Address
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
}