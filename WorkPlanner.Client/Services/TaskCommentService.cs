using System.Net.Http.Json;
using WorkPlanner.Client.Models;

namespace WorkPlanner.Client.Services;

public class TaskCommentService
{
    private readonly HttpClient _httpClient;

    public TaskCommentService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<TaskComment>> GetCommentsAsync(int taskId)
    {
        var response = await _httpClient.GetAsync($"api/tasks/{taskId}/comments");
        if (!response.IsSuccessStatusCode)
        {
            return new List<TaskComment>();
        }

        return await response.Content.ReadFromJsonAsync<List<TaskComment>>() ?? new List<TaskComment>();
    }

    public async Task<TaskComment?> CreateCommentAsync(int taskId, CreateTaskCommentRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/tasks/{taskId}/comments", request);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<TaskComment>();
    }
}
