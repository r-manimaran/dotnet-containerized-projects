namespace BusinesscardsApi.Models;

public class BusinessCard
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Title { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string CompanyName { get; set; }
    public string Website { get; set; }
    public string Address { get; set; }
    public string ProfileImageUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? GitHubUrl { get; set; }
    public string? PersonalBlogUrl { get; set; }
}
