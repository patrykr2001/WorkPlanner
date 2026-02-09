using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using WorkPlanner.Client.Models;
using WorkPlanner.Client.Services;
using TaskStatus = WorkPlanner.Client.Models.TaskStatus;

namespace WorkPlanner.Client.Pages;

public partial class Kanban : ComponentBase
{
    [Inject] private ProjectService ProjectService { get; set; } = null!;
    [Inject] private SprintService SprintService { get; set; } = null!;
    [Inject] private TaskService TaskService { get; set; } = null!;
    [Inject] private IJSRuntime JsRuntime { get; set; } = null!;

    private const string SelectedProjectStorageKey = "workplanner.selectedProjectId";
    protected List<Project> Projects { get; private set; } = new();
    protected int SelectedProjectId
    {
        get => _selectedProjectId;
        set
        {
            if (_selectedProjectId == value)
            {
                return;
            }

            _selectedProjectId = value;
            _ = SaveSelectedProjectAsync();
        }
    }

    private int _selectedProjectId;

    protected List<Sprint> Sprints { get; private set; } = new();
    protected int SelectedSprintId
    {
        get => _selectedSprintId;
        set
        {
            if (_selectedSprintId == value)
            {
                return;
            }

            _selectedSprintId = value;
            _ = SaveSelectedSprintAsync();
        }
    }

    private int _selectedSprintId;
    protected List<TaskItem> SprintTasks { get; private set; } = new();
    private TaskItem? _draggedTask;
    protected bool IsUnauthorized { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        Projects = await ProjectService.GetProjectsAsync();
        IsUnauthorized = Projects.Count == 0;

        var storedProjectId = await JsRuntime.InvokeAsync<string>("localStorage.getItem", SelectedProjectStorageKey);
        if (int.TryParse(storedProjectId, out var projectId) && Projects.Any(p => p.Id == projectId))
        {
            SelectedProjectId = projectId;
        }
        else if (Projects.Count > 0)
        {
            SelectedProjectId = Projects[0].Id;
            await JsRuntime.InvokeVoidAsync("localStorage.setItem", SelectedProjectStorageKey, SelectedProjectId.ToString());
        }

        if (SelectedProjectId > 0)
        {
            await LoadSprintsAsync();
        }
    }

    private async Task SaveSelectedProjectAsync()
    {
        if (SelectedProjectId <= 0)
        {
            return;
        }

        await JsRuntime.InvokeVoidAsync("localStorage.setItem", SelectedProjectStorageKey, SelectedProjectId.ToString());
        await LoadSprintsAsync();
    }

    private async Task LoadSprintsAsync()
    {
        Sprints = await SprintService.GetSprintsAsync(SelectedProjectId, includeArchived: false);

        var activeSprint = Sprints.FirstOrDefault(s => s.IsActive);
        if (activeSprint != null)
        {
            SelectedSprintId = activeSprint.Id;
        }
        else if (Sprints.Count > 0)
        {
            SelectedSprintId = Sprints[0].Id;
        }
        else
        {
            SelectedSprintId = 0;
        }

        await LoadSprintTasksAsync();
    }

    private async Task SaveSelectedSprintAsync()
    {
        if (SelectedSprintId <= 0)
        {
            SprintTasks = new List<TaskItem>();
            StateHasChanged();
            return;
        }

        await LoadSprintTasksAsync();
    }

    private async Task LoadSprintTasksAsync()
    {
        if (SelectedSprintId <= 0)
        {
            SprintTasks = new List<TaskItem>();
            return;
        }

        SprintTasks = await TaskService.GetTasksAsync(SelectedProjectId, SelectedSprintId);
        IsUnauthorized = Projects.Count == 0 && Sprints.Count == 0 && SprintTasks.Count == 0;
        StateHasChanged();
    }

    protected void OnDragStart(TaskItem task)
    {
        _draggedTask = task;
    }

    protected async Task OnDropToColumn(TaskStatus status)
    {
        if (_draggedTask == null)
        {
            return;
        }

        if (IsUnauthorized)
        {
            return;
        }

        var tasksInColumn = SprintTasks.Where(t => t.Status == status).ToList();
        var request = new MoveTaskRequest
        {
            SprintId = SelectedSprintId,
            Status = status,
            NewOrder = tasksInColumn.Count
        };

        await TaskService.MoveTaskAsync(_draggedTask.Id, request);
        await LoadSprintTasksAsync();
        _draggedTask = null;
    }
}
