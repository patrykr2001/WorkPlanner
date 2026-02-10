namespace WorkPlanner.Client.Models;

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string OwnerId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsArchived { get; set; }
    public string? EnabledStatuses { get; set; }
}

public class CreateProjectRequest
{
    public string Name { get; set; } = string.Empty;
    public string? EnabledStatuses { get; set; }
}

public class UpdateProjectRequest
{
    public string Name { get; set; } = string.Empty;
    public bool IsArchived { get; set; }
    public string? EnabledStatuses { get; set; }
}

public class ProjectMember
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public ProjectRole Role { get; set; }

    public string FullName => string.IsNullOrWhiteSpace(FirstName) && string.IsNullOrWhiteSpace(LastName)
        ? Email
        : $"{FirstName} {LastName}".Trim();
}

public enum ProjectRole
{
    Owner = 0,
    Member = 1
}

public class AddProjectMemberRequest
{
    public string Email { get; set; } = string.Empty;
}
