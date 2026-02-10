namespace WorkPlanner.Client.Models;

public class TaskComment
{
    public int Id { get; set; }
    public int TaskItemId { get; set; }
    public string AuthorId { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorEmail { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateTaskCommentRequest
{
    public string Body { get; set; } = string.Empty;
}
