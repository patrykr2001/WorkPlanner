using System.Net.Http.Json;
using WorkPlanner.Client.Models;

namespace WorkPlanner.Client.Services;

public class ProjectService
{
    private readonly HttpClient _httpClient;

    public ProjectService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Project>> GetProjectsAsync()
    {
        var response = await _httpClient.GetAsync("api/projects");
        if (!response.IsSuccessStatusCode)
        {
            return new List<Project>();
        }

        return await response.Content.ReadFromJsonAsync<List<Project>>() ?? new List<Project>();
    }

    public async Task<Project?> GetProjectAsync(int id)
    {
        var response = await _httpClient.GetAsync($"api/projects/{id}");
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<Project>();
    }

    public async Task<Project> CreateProjectAsync(CreateProjectRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/projects", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Project>() ?? new Project();
    }
}
