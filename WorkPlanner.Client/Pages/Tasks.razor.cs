using Microsoft.AspNetCore.Components;
using MudBlazor;
using WorkPlanner.Client.Models;
using WorkPlanner.Client.Services;
using TaskStatus = WorkPlanner.Client.Models.TaskStatus;

namespace WorkPlanner.Client.Pages;

public partial class Tasks : ComponentBase
{
    [Inject] private TaskService TaskService { get; set; } = null!;
    [Inject] private ProjectService ProjectService { get; set; } = null!;
    [Inject] private SprintService SprintService { get; set; } = null!;
    [Inject] private AuthService AuthService { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;

    protected List<Project> Projects { get; private set; } = new();
    protected List<Sprint> Sprints { get; private set; } = new();
    protected List<TaskItem> MyTasks { get; private set; } = new();
    protected List<TaskStatus> AvailableStatuses { get; private set; } = new();

    protected int SelectedProjectId { get; set; }
    protected int SelectedSprintId { get; set; }
    protected TaskStatus SelectedStatus { get; set; } = TaskStatus.Todo;

    protected List<TaskItem> FilteredTasks => MyTasks
        .Where(t => SelectedProjectId == 0 || t.ProjectId == SelectedProjectId)
        .Where(t => SelectedSprintId == 0 || t.SprintId == SelectedSprintId)
        .Where(t => t.Status == SelectedStatus)
        .OrderBy(t => t.Order)
        .ToList();

    protected override async Task OnInitializedAsync()
    {
        Projects = await ProjectService.GetProjectsAsync();
        AvailableStatuses = GetAvailableStatusesForProject(null)
            .Where(status => status != TaskStatus.Backlog)
            .ToList();
        if (AvailableStatuses.Count > 0)
        {
            SelectedStatus = AvailableStatuses[0];
        }
        await LoadTasksAsync();
    }

    protected async Task OnFiltersChanged()
    {
        await LoadTasksAsync();
    }

    protected async Task LoadTasksAsync()
    {
        var projectId = SelectedProjectId == 0 ? (int?)null : SelectedProjectId;
        var sprintId = SelectedSprintId == 0 ? (int?)null : SelectedSprintId;

        if (projectId.HasValue)
        {
            Sprints = await SprintService.GetSprintsAsync(projectId.Value, includeArchived: false);
            var project = Projects.FirstOrDefault(p => p.Id == projectId.Value);
        AvailableStatuses = GetAvailableStatusesForProject(project?.EnabledStatuses)
            .Where(status => status != TaskStatus.Backlog)
            .ToList();
        }
        else
        {
            Sprints = new List<Sprint>();
        AvailableStatuses = GetAvailableStatusesForProject(null)
            .Where(status => status != TaskStatus.Backlog)
            .ToList();
        }

        var tasks = await TaskService.GetTasksAsync(projectId, sprintId);
        var currentUserId = AuthService.CurrentUser?.Id;

        MyTasks = string.IsNullOrWhiteSpace(currentUserId)
            ? new List<TaskItem>()
            : tasks.Where(t => t.AssigneeId == currentUserId).ToList();

        if (!AvailableStatuses.Contains(SelectedStatus) && AvailableStatuses.Count > 0)
        {
            SelectedStatus = AvailableStatuses[0];
        }
    }

    protected async Task OpenNewTaskDialog()
    {
        var dialog = await DialogService.ShowAsync<global::WorkPlanner.Client.Shared.TaskDialog>("New task", new DialogOptions
        {
            MaxWidth = MaxWidth.ExtraLarge,
            FullWidth = true
        });
        var result = await dialog.Result;

        if (result is { Canceled: false })
        {
            await LoadTasksAsync();
        }
    }

    protected string GetProjectName(int projectId)
    {
        return Projects.FirstOrDefault(p => p.Id == projectId)?.Name ?? "-";
    }

    protected string GetSprintName(int? sprintId)
    {
        if (!sprintId.HasValue)
        {
            return "-";
        }

        return Sprints.FirstOrDefault(s => s.Id == sprintId.Value)?.Name ?? "-";
    }

    protected async Task OpenTaskDialog(TaskItem task)
    {
        var parameters = new DialogParameters
        {
            ["InitialTask"] = task
        };

        var dialog = await DialogService.ShowAsync<global::WorkPlanner.Client.Shared.TaskDialog>("Task", parameters, new DialogOptions
        {
            MaxWidth = MaxWidth.ExtraLarge,
            FullWidth = true
        });
        var result = await dialog.Result;

        if (result is { Canceled: false })
        {
            await LoadTasksAsync();
        }
    }

    protected string GetStatusLabel(TaskStatus status)
    {
        return status switch
        {
            TaskStatus.InProgress => "In Progress",
            TaskStatus.Refine => "Refine",
            TaskStatus.Review => "Review",
            _ => status.ToString()
        };
    }

    private static List<TaskStatus> GetAvailableStatusesForProject(string? enabledStatuses)
    {
        var baseStatuses = new List<TaskStatus>
        {
            TaskStatus.Backlog,
            TaskStatus.Todo,
            TaskStatus.InProgress,
            TaskStatus.Done
        };

        if (string.IsNullOrWhiteSpace(enabledStatuses))
        {
            return baseStatuses;
        }

        var parsed = enabledStatuses
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(s => Enum.TryParse<TaskStatus>(s, true, out var value) ? value : (TaskStatus?)null)
            .Where(v => v.HasValue)
            .Select(v => v!.Value)
            .Distinct()
            .ToList();

        if (!parsed.Contains(TaskStatus.Backlog))
        {
            parsed.Add(TaskStatus.Backlog);
        }
        if (!parsed.Contains(TaskStatus.Todo))
        {
            parsed.Add(TaskStatus.Todo);
        }
        if (!parsed.Contains(TaskStatus.InProgress))
        {
            parsed.Add(TaskStatus.InProgress);
        }
        if (!parsed.Contains(TaskStatus.Done))
        {
            parsed.Add(TaskStatus.Done);
        }

        var order = new[]
        {
            TaskStatus.Backlog,
            TaskStatus.Refine,
            TaskStatus.Todo,
            TaskStatus.InProgress,
            TaskStatus.Review,
            TaskStatus.Done
        };

        return parsed.OrderBy(status => Array.IndexOf(order, status)).ToList();
    }
}
