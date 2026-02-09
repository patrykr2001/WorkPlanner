namespace WorkPlanner.Client.Models;

public class MoveTaskRequest
{
    public int? SprintId { get; set; }
    public TaskStatus Status { get; set; }
    public int NewOrder { get; set; }
}
