namespace WorkPlanner.Client.Models;

public class TaskItem
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskStatus Status { get; set; } = TaskStatus.Backlog;
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? AssigneeId { get; set; }
    public int? SprintId { get; set; }
    public int Order { get; set; }
}

public enum TaskStatus
{
    Backlog = 0,
    Todo = 1,
    InProgress = 2,
    Done = 3,
    Review = 4,
    Refine = 5
}
