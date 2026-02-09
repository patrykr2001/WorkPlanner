namespace WorkPlanner.Client.Models;

public class WorkEntry
{
    public int Id { get; set; }
    public int TaskItemId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public TimeSpan? Duration => EndTime.HasValue ? EndTime.Value - StartTime : null;
    public TaskItem? TaskItem { get; set; }
}
