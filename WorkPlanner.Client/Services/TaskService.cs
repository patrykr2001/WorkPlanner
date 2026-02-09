using System.Net.Http.Json;
using WorkPlanner.Client.Models;

namespace WorkPlanner.Client.Services;

public class TaskService
{
    private readonly HttpClient _httpClient;

    public TaskService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<TaskItem>> GetTasksAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<TaskItem>>("api/tasks") ?? new List<TaskItem>();
    }

    public async Task<TaskItem?> GetTaskAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<TaskItem>($"api/tasks/{id}");
    }

    public async Task<TaskItem> CreateTaskAsync(TaskItem task)
    {
        var response = await _httpClient.PostAsJsonAsync("api/tasks", task);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TaskItem>() ?? task;
    }

    public async Task UpdateTaskAsync(TaskItem task)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/tasks/{task.Id}", task);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteTaskAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/tasks/{id}");
        response.EnsureSuccessStatusCode();
    }
}
