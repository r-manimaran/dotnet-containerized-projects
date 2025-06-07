using System.ComponentModel.DataAnnotations;

namespace AppreciateAppApi.Models;

public class Item
{
    public int Id { get; set; }
    [Required]
    [MaxLength(500)]
    public string Content { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public int SenderId { get; set; }
    public Employee Sender { get; set; }
    public List<Employee> Receivers { get; set; } = new(); 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
