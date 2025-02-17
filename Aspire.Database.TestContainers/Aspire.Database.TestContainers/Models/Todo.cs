namespace Aspire.Database.TestContainers.Models;

public class Todo
{
    public int Id { get; set; }
    public string Task { get; set; }
    public bool IsCompleted { get; set; } = false;
    public DateTime? DueDate { get; set; }
}
