using Microsoft.AspNetCore.Components;
using WorkPlanner.Client.Models;
using WorkPlanner.Client.Services;

namespace WorkPlanner.Client.Pages;

public partial class Projects : ComponentBase
{
    [Inject] private ProjectService ProjectService { get; set; } = null!;

    protected List<Project> ProjectList { get; private set; } = new();
    protected List<Project> FilteredProjects => ShowArchived
        ? ProjectList
        : ProjectList.Where(p => !p.IsArchived).ToList();

    protected string NewProjectName { get; set; } = string.Empty;
    protected string NewProjectStatuses { get; set; } = string.Empty;
    protected bool ShowArchived { get; set; }

    protected int? EditingProjectId { get; set; }
    protected string EditProjectName { get; set; } = string.Empty;
    protected string EditProjectStatuses { get; set; } = string.Empty;

    protected Project? SelectedProject { get; set; }
    protected List<ProjectMember> Members { get; private set; } = new();
    protected string NewMemberEmail { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await LoadProjectsAsync();
    }

    protected async Task LoadProjectsAsync()
    {
        ProjectList = await ProjectService.GetProjectsAsync();
        foreach (var project in ProjectList)
        {
            if (string.IsNullOrWhiteSpace(project.EnabledStatuses))
            {
                project.EnabledStatuses = "Todo,InProgress,Done";
            }
        }
        StateHasChanged();
    }

    protected async Task CreateProject()
    {
        if (string.IsNullOrWhiteSpace(NewProjectName))
        {
            return;
        }

        var statuses = NormalizeStatuses(NewProjectStatuses);
        if (string.IsNullOrWhiteSpace(statuses))
        {
            statuses = "Todo,InProgress,Done";
        }

        await ProjectService.CreateProjectAsync(new CreateProjectRequest
        {
            Name = NewProjectName.Trim(),
            EnabledStatuses = statuses
        });
        NewProjectName = string.Empty;
        NewProjectStatuses = string.Empty;
        await LoadProjectsAsync();
    }

    protected void StartEdit(Project project)
    {
        EditingProjectId = project.Id;
        EditProjectName = project.Name;
        EditProjectStatuses = NormalizeStatuses(project.EnabledStatuses ?? string.Empty);
    }

    protected void CancelEdit()
    {
        EditingProjectId = null;
        EditProjectName = string.Empty;
        EditProjectStatuses = string.Empty;
    }

    protected async Task SaveProject(Project project)
    {
        if (EditingProjectId != project.Id)
        {
            return;
        }

        var name = string.IsNullOrWhiteSpace(EditProjectName) ? project.Name : EditProjectName.Trim();
        var statuses = NormalizeStatuses(EditProjectStatuses);
        if (string.IsNullOrWhiteSpace(statuses))
        {
            statuses = "Todo,InProgress,Done";
        }

        await ProjectService.UpdateProjectAsync(project.Id, new UpdateProjectRequest
        {
            Name = name,
            IsArchived = project.IsArchived,
            EnabledStatuses = statuses
        });

        EditingProjectId = null;
        EditProjectName = string.Empty;
        EditProjectStatuses = string.Empty;
        await LoadProjectsAsync();
    }

    protected async Task ToggleArchive(Project project)
    {
        var statuses = NormalizeStatuses(project.EnabledStatuses ?? string.Empty);
        if (string.IsNullOrWhiteSpace(statuses))
        {
            statuses = "Todo,InProgress,Done";
        }

        await ProjectService.UpdateProjectAsync(project.Id, new UpdateProjectRequest
        {
            Name = project.Name,
            IsArchived = !project.IsArchived,
            EnabledStatuses = statuses
        });

        await LoadProjectsAsync();
    }

    protected async Task SelectProject(Project project)
    {
        SelectedProject = project;
        Members = await ProjectService.GetMembersAsync(project.Id);
        NewMemberEmail = string.Empty;
    }

    protected async Task AddMember()
    {
        if (SelectedProject == null || string.IsNullOrWhiteSpace(NewMemberEmail))
        {
            return;
        }

        await ProjectService.AddMemberAsync(SelectedProject.Id, new AddProjectMemberRequest
        {
            Email = NewMemberEmail.Trim()
        });

        NewMemberEmail = string.Empty;
        Members = await ProjectService.GetMembersAsync(SelectedProject.Id);
    }

    protected async Task RemoveMember(ProjectMember member)
    {
        if (SelectedProject == null)
        {
            return;
        }

        await ProjectService.RemoveMemberAsync(SelectedProject.Id, member.UserId);
        Members = await ProjectService.GetMembersAsync(SelectedProject.Id);
    }

    protected string GetOwnerLabel(Project project)
    {
        if (SelectedProject != null && SelectedProject.Id == project.Id)
        {
            var owner = Members.FirstOrDefault(m => m.UserId == project.OwnerId);
            if (owner != null)
            {
                return string.IsNullOrWhiteSpace(owner.FullName) ? owner.Email : owner.FullName;
            }
        }

        return project.OwnerId;
    }

    protected string GetProjectStatuses(Project project)
    {
        var normalized = NormalizeStatuses(project.EnabledStatuses ?? string.Empty);
        return string.IsNullOrWhiteSpace(normalized) ? "Todo,InProgress,Done" : normalized;
    }

    private static string NormalizeStatuses(string statuses)
    {
        if (string.IsNullOrWhiteSpace(statuses))
        {
            return string.Empty;
        }

        var order = new[]
        {
            Models.TaskStatus.Refine.ToString(),
            Models.TaskStatus.Todo.ToString(),
            Models.TaskStatus.InProgress.ToString(),
            Models.TaskStatus.Review.ToString(),
            Models.TaskStatus.Done.ToString()
        };

        var allowed = new HashSet<string>(order, StringComparer.OrdinalIgnoreCase);
        var normalized = statuses
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(s => allowed.Contains(s))
            .Select(s => order.First(o => o.Equals(s, StringComparison.OrdinalIgnoreCase)))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        return string.Join(',', normalized);
    }
}
