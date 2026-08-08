using HamperStore.Web.Services;
using HamperStore.Core.Interfaces;
using HamperStore.Infrastructure.Data;
using HamperStore.Infrastructure.Repositories;
using HamperStore.Infrastructure.Services;
using HamperStore.Web.Components;
using Microsoft.EntityFrameworkCore;
using System.IO;

// Load local .env file if it exists (for local development variables)
var currentDirectory = Directory.GetCurrentDirectory();
var envPath = Path.Combine(currentDirectory, ".env");
if (!File.Exists(envPath))
{
    envPath = Path.Combine(Directory.GetParent(currentDirectory)?.FullName ?? currentDirectory, ".env");
}

if (File.Exists(envPath))
{
    foreach (var line in File.ReadAllLines(envPath))
    {
        if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
            continue;

        var parts = line.Split('=', 2);
        if (parts.Length == 2)
        {
            var key = parts[0].Trim();
            var value = parts[1].Trim();
            if ((value.StartsWith("\"") && value.EndsWith("\"")) || (value.StartsWith("'") && value.EndsWith("'")))
            {
                value = value.Substring(1, value.Length - 2);
            }
            Environment.SetEnvironmentVariable(key, value);
        }
    }
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<IHamperRepository, HamperRepository>();
builder.Services.AddScoped<IInquiryRepository, InquiryRepository>();
builder.Services.AddScoped<IInquiryService, InquiryService>();
builder.Services.AddScoped<UserSessionService>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddSingleton<GitHubSyncService>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<GitHubSyncService>());

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var syncService = scope.ServiceProvider.GetRequiredService<GitHubSyncService>();
    syncService.InitializeDatabaseAndAssetsAsync().GetAwaiter().GetResult();

    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DbSeeder.Seed(db);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
