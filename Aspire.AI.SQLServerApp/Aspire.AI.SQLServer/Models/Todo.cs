namespace Aspire.AI.SQLServer.Models;

public class Todo
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public TodoMetadata Metadata { get; set; } = new();
}

public enum TodoPriority
{
    Low,
    Medium,
    High
}
public class  TodoMetadata
{
 public TodoPriority Priority { get; set; }

 public List<string> Tags { get; set; } = new();
    public CreatedByInfo CreatedBy { get; set; }
}

public class  CreatedByInfo
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
