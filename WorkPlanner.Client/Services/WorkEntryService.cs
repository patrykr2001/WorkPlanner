using System.Net.Http.Json;
using WorkPlanner.Client.Models;

namespace WorkPlanner.Client.Services;

public class WorkEntryService
{
    private readonly HttpClient _httpClient;

    public WorkEntryService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<WorkEntry>> GetWorkEntriesAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<WorkEntry>>("api/workentries") ?? new List<WorkEntry>();
    }

    public async Task<List<WorkEntry>> GetWorkEntriesByTaskAsync(int taskId)
    {
        return await _httpClient.GetFromJsonAsync<List<WorkEntry>>($"api/workentries/by-task/{taskId}") ?? new List<WorkEntry>();
    }

    public async Task<WorkEntry> CreateWorkEntryAsync(WorkEntry entry)
    {
        var response = await _httpClient.PostAsJsonAsync("api/workentries", entry);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<WorkEntry>() ?? entry;
    }

    public async Task UpdateWorkEntryAsync(WorkEntry entry)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/workentries/{entry.Id}", entry);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteWorkEntryAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/workentries/{id}");
        response.EnsureSuccessStatusCode();
    }
}
