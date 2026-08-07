using HamperStore.Web.Services;
using HamperStore.Core.Interfaces;
using HamperStore.Infrastructure.Data;
using HamperStore.Infrastructure.Repositories;
using HamperStore.Infrastructure.Services;
using HamperStore.Web.Components;
using Microsoft.EntityFrameworkCore;

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


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
