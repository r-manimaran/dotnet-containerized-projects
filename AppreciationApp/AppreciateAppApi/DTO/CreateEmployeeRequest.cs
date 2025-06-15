namespace AppreciateAppApi.DTO;

public class CreateEmployeeRequest
{
    // public string UserName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    //public string Email { get; set; } = string.Empty;

    // Add this property for file upload
    public IFormFile? ProfileImage { get; set; }
}
