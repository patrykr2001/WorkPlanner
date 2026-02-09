using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using WorkPlanner.Client;
using WorkPlanner.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Load configuration from appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

// HttpClient with cookies enabled - read BaseAddress from configuration
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:7191";
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(apiBaseUrl),
    DefaultRequestHeaders = { { "Accept", "application/json" } }
});

// MudBlazor
builder.Services.AddMudServices();

// Custom services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<TaskService>();
builder.Services.AddScoped<WorkEntryService>();

var app = builder.Build();

// Initialize auth state
var authService = app.Services.GetRequiredService<AuthService>();
await authService.RefreshUserAsync();

await app.RunAsync();
