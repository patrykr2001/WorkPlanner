namespace WorkPlanner.Client.Models;

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string OwnerId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsArchived { get; set; }
}

public class CreateProjectRequest
{
    public string Name { get; set; } = string.Empty;
}
