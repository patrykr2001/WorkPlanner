using System.Net.Http.Json;
using WorkPlanner.Client.Models;

namespace WorkPlanner.Client.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private UserInfo? _currentUser;
    private bool _isAuthenticated;

    public event Action? OnAuthStateChanged;

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public bool IsAuthenticated => _isAuthenticated;
    public UserInfo? CurrentUser => _currentUser;

    public async Task<bool> LoginAsync(string email, string password, bool rememberMe = true)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", new
            {
                Email = email,
                Password = password,
                RememberMe = rememberMe
            });

            if (response.IsSuccessStatusCode)
            {
                await RefreshUserAsync();
                return true;
            }
        }
        catch (Exception)
        {
            // Log error
        }

        return false;
    }

    public async Task<bool> RegisterAsync(RegisterModel model)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", model);

            if (response.IsSuccessStatusCode)
            {
                await RefreshUserAsync();
                return true;
            }
        }
        catch (Exception)
        {
            // Log error
        }

        return false;
    }

    public async Task LogoutAsync()
    {
        try
        {
            await _httpClient.PostAsync("api/auth/logout", null);
        }
        catch (Exception)
        {
            // Log error
        }
        finally
        {
            _currentUser = null;
            _isAuthenticated = false;
            NotifyAuthStateChanged();
        }
    }

    public async Task<bool> RefreshUserAsync()
    {
        try
        {
            var user = await _httpClient.GetFromJsonAsync<UserInfo>("api/auth/me");
            if (user != null)
            {
                _currentUser = user;
                _isAuthenticated = true;
                NotifyAuthStateChanged();
                return true;
            }
        }
        catch (Exception)
        {
            // User not authenticated
        }

        _currentUser = null;
        _isAuthenticated = false;
        NotifyAuthStateChanged();
        return false;
    }

    private void NotifyAuthStateChanged()
    {
        OnAuthStateChanged?.Invoke();
    }
}
