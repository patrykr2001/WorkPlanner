using System.Net.Http.Json;
using WorkPlanner.Client.Models;

namespace WorkPlanner.Client.Services;

public class SprintService
{
    private readonly HttpClient _httpClient;

    public SprintService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Sprint>> GetSprintsAsync(int projectId, bool includeArchived = false)
    {
        var url = $"api/projects/{projectId}/sprints?includeArchived={includeArchived.ToString().ToLowerInvariant()}";
        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            return new List<Sprint>();
        }

        return await response.Content.ReadFromJsonAsync<List<Sprint>>() ?? new List<Sprint>();
    }

    public async Task<Sprint> CreateSprintAsync(int projectId, CreateSprintRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/projects/{projectId}/sprints", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Sprint>() ?? new Sprint();
    }

    public async Task UpdateSprintAsync(int projectId, int sprintId, UpdateSprintRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/projects/{projectId}/sprints/{sprintId}", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task ActivateSprintAsync(int projectId, int sprintId)
    {
        var response = await _httpClient.PostAsync($"api/projects/{projectId}/sprints/{sprintId}/activate", null);
        response.EnsureSuccessStatusCode();
    }

    public async Task ArchiveSprintAsync(int projectId, int sprintId)
    {
        var response = await _httpClient.PostAsync($"api/projects/{projectId}/sprints/{sprintId}/archive", null);
        response.EnsureSuccessStatusCode();
    }
}
