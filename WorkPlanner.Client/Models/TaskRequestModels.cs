namespace WorkPlanner.Client.Models;

public class CreateTaskRequest
{
    public int ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskStatus Status { get; set; } = TaskStatus.Backlog;
    public string? AssigneeId { get; set; }
    public int? SprintId { get; set; }
    public int Order { get; set; }
}

public class UpdateTaskRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskStatus Status { get; set; }
    public string? AssigneeId { get; set; }
    public int? SprintId { get; set; }
    public int Order { get; set; }
}
