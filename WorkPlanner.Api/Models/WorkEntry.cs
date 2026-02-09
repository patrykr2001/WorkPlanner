namespace WorkPlanner.Api.Models;

public class WorkEntry
{
    public int Id { get; set; }
    public int TaskItemId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public TaskItem TaskItem { get; set; } = null!;
    
    public TimeSpan? Duration => EndTime.HasValue ? EndTime.Value - StartTime : null;
}
