namespace WorkPlanner.Api.Models;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskStatus Status { get; set; } = TaskStatus.Todo;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    
    public ICollection<WorkEntry> WorkEntries { get; set; } = new List<WorkEntry>();
}

public enum TaskStatus
{
    Todo,
    InProgress,
    Done
}
