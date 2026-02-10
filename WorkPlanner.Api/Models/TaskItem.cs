namespace WorkPlanner.Api.Models;

public class TaskItem
{
    public int Id { get; set; }
    public int? ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskStatus Status { get; set; } = TaskStatus.Todo;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public int? SprintId { get; set; }
    public int Order { get; set; }

    public string? AssigneeId { get; set; }
    
    public ICollection<WorkEntry> WorkEntries { get; set; } = new List<WorkEntry>();
    public ICollection<TaskComment> Comments { get; set; } = new List<TaskComment>();
    public Project? Project { get; set; }
    public Sprint? Sprint { get; set; }
    public ApplicationUser? Assignee { get; set; }
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
