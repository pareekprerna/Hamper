using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using HamperStore.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HamperStore.Web.Services
{
    public class GitHubSyncService : BackgroundService
    {
        private readonly ILogger<GitHubSyncService> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly HttpClient _client;
        private readonly string? _token;
        private readonly string? _username;
        private readonly string? _repo;
        private readonly string _branch;
        private readonly string _dbPath;

        private readonly object _syncLock = new();
        private bool _isSyncPending = false;
        private DateTime? _nextSyncTime = null;

        public GitHubSyncService(
            ILogger<GitHubSyncService> logger,
            IConfiguration configuration,
            IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
            _client = new HttpClient();
            _client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("HamperStore-Sync", "1.0"));

            // Read settings from environment variables or appsettings
            _token = configuration["GITHUB_TOKEN"] ?? Environment.GetEnvironmentVariable("GITHUB_TOKEN");
            _username = configuration["GITHUB_USERNAME"] ?? Environment.GetEnvironmentVariable("GITHUB_USERNAME");
            _repo = configuration["GITHUB_REPO"] ?? Environment.GetEnvironmentVariable("GITHUB_REPO");
            _branch = configuration["GITHUB_BRANCH"] ?? Environment.GetEnvironmentVariable("GITHUB_BRANCH") ?? "db-state";

            _dbPath = Path.Combine(Directory.GetCurrentDirectory(), "HamperStore.db");

            if (IsConfigured())
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
                _logger.LogInformation("GitHub Database Sync Service is configured for repo: {Username}/{Repo} on branch {Branch}", _username, _repo, _branch);
            }
            else
            {
                _logger.LogWarning("GitHub Database Sync Service is NOT configured. Set GITHUB_TOKEN, GITHUB_USERNAME, and GITHUB_REPO to enable free cloud persistence.");
            }
        }

        public bool IsConfigured()
        {
            return !string.IsNullOrEmpty(_token) && !string.IsNullOrEmpty(_username) && !string.IsNullOrEmpty(_repo);
        }

        public async Task InitializeDatabaseAndAssetsAsync()
        {
            if (!IsConfigured()) return;

            try
            {
                _logger.LogInformation("Checking for database and assets in GitHub repository...");

                // 1. Check and download database file
                var dbMeta = await GetGitHubFileMetadataAsync("HamperStore.db");
                if (dbMeta != null && !string.IsNullOrEmpty(dbMeta.DownloadUrl))
                {
                    _logger.LogInformation("Found database file on GitHub. Downloading...");
                    var dbBytes = await _client.GetByteArrayAsync(dbMeta.DownloadUrl);
                    await File.WriteAllBytesAsync(_dbPath, dbBytes);
                    _logger.LogInformation("Database file successfully downloaded and restored.");
                }
                else
                {
                    _logger.LogInformation("No existing database file found on GitHub. A new one will be created and seeded.");
                }

                // 2. Check and download uploaded images directory
                var uploadsPath = Path.Combine(_env.WebRootPath, "images", "hampers", "uploads");
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }

                var files = await GetGitHubDirectoryContentsAsync("wwwroot/images/hampers/uploads");
                if (files != null && files.Any())
                {
                    _logger.LogInformation("Found {Count} uploaded images on GitHub. Downloading missing assets...", files.Count);
                    foreach (var file in files)
                    {
                        var localFilePath = Path.Combine(uploadsPath, file.Name);
                        if (!File.Exists(localFilePath) && !string.IsNullOrEmpty(file.DownloadUrl))
                        {
                            var imgBytes = await _client.GetByteArrayAsync(file.DownloadUrl);
                            await File.WriteAllBytesAsync(localFilePath, imgBytes);
                            _logger.LogInformation("Restored image asset: {Name}", file.Name);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during GitHub startup download synchronization.");
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!IsConfigured()) return;

            // Subscribe to database changes
            AppDbContext.DatabaseChanged += OnDatabaseChanged;

            _logger.LogInformation("GitHub Sync Background worker started.");

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    bool runSync = false;

                    lock (_syncLock)
                    {
                        if (_isSyncPending && _nextSyncTime.HasValue && DateTime.UtcNow >= _nextSyncTime.Value)
                        {
                            runSync = true;
                            _isSyncPending = false;
                            _nextSyncTime = null;
                        }
                    }

                    if (runSync)
                    {
                        await PerformDbSyncAsync();
                    }

                    // Check every 1 second
                    await Task.Delay(1000, stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                // Normal shutdown
            }
            finally
            {
                AppDbContext.DatabaseChanged -= OnDatabaseChanged;
            }
        }

        private void OnDatabaseChanged(object? sender, EventArgs e)
        {
            lock (_syncLock)
            {
                _isSyncPending = true;
                // Debounce: Sync 5 seconds after the last modification
                _nextSyncTime = DateTime.UtcNow.AddSeconds(5);
                _logger.LogDebug("Database modification detected. Scheduled sync at {Time}", _nextSyncTime);
            }
        }

        private async Task PerformDbSyncAsync()
        {
            if (!File.Exists(_dbPath))
            {
                _logger.LogWarning("Local database file not found. Skipping sync.");
                return;
            }

            try
            {
                _logger.LogInformation("Syncing local SQLite database changes to GitHub...");

                // Read local database safely by copying it to avoid write locks
                byte[] dbBytes;
                var tempPath = Path.GetTempFileName();
                try
                {
                    File.Copy(_dbPath, tempPath, true);
                    dbBytes = await File.ReadAllBytesAsync(tempPath);
                }
                finally
                {
                    if (File.Exists(tempPath)) File.Delete(tempPath);
                }

                await PushFileToGitHubAsync("HamperStore.db", dbBytes, "Auto-sync database state");
                _logger.LogInformation("Database successfully synced to GitHub branch {Branch}.", _branch);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to sync SQLite database to GitHub.");
            }
        }

        public async Task SyncFileAsync(string relativePath, byte[] content)
        {
            if (!IsConfigured()) return;

            try
            {
                // Normalize path to use forward slashes for GitHub
                var githubPath = relativePath.Replace("\\", "/");
                _logger.LogInformation("Syncing asset '{Path}' to GitHub...", githubPath);
                await PushFileToGitHubAsync(githubPath, content, $"Upload asset: {Path.GetFileName(relativePath)}");
                _logger.LogInformation("Asset '{Path}' successfully synced to GitHub.", githubPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to sync asset '{Path}' to GitHub.", relativePath);
            }
        }

        private async Task PushFileToGitHubAsync(string path, byte[] content, string commitMessage)
        {
            var url = $"https://api.github.com/repos/{_username}/{_repo}/contents/{path}";

            // 1. Get SHA if file already exists
            string? sha = null;
            var meta = await GetGitHubFileMetadataAsync(path);
            if (meta != null)
            {
                sha = meta.Sha;
            }

            // 2. Put file content
            var base64Content = Convert.ToBase64String(content);
            var payload = new JsonObject
            {
                ["message"] = commitMessage,
                ["content"] = base64Content,
                ["branch"] = _branch
            };

            if (!string.IsNullOrEmpty(sha))
            {
                payload["sha"] = sha;
            }

            var requestJson = JsonSerializer.Serialize(payload);
            using var request = new HttpRequestMessage(HttpMethod.Put, url);
            request.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            using var response = await _client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"GitHub API PUT failed: {response.StatusCode}. Details: {responseContent}");
            }
        }

        private async Task<GitHubFileMetadata?> GetGitHubFileMetadataAsync(string path)
        {
            var url = $"https://api.github.com/repos/{_username}/{_repo}/contents/{path}?ref={_branch}";

            using var response = await _client.GetAsync(url);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                var errContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("GitHub API GET metadata returned error for '{Path}': {Status}. Details: {Details}", path, response.StatusCode, errContent);
                return null;
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);
            var root = doc.RootElement;

            return new GitHubFileMetadata
            {
                Name = root.GetProperty("name").GetString() ?? "",
                Sha = root.GetProperty("sha").GetString() ?? "",
                DownloadUrl = root.TryGetProperty("download_url", out var dlUrl) ? dlUrl.GetString() : null
            };
        }

        private async Task<List<GitHubFileMetadata>?> GetGitHubDirectoryContentsAsync(string path)
        {
            var url = $"https://api.github.com/repos/{_username}/{_repo}/contents/{path}?ref={_branch}";

            using var response = await _client.GetAsync(url);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                var errContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("GitHub API GET directory contents returned error for '{Path}': {Status}. Details: {Details}", path, response.StatusCode, errContent);
                return null;
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);
            var list = new List<GitHubFileMetadata>();

            if (doc.RootElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var el in doc.RootElement.EnumerateArray())
                {
                    var type = el.GetProperty("type").GetString();
                    if (type == "file")
                    {
                        list.Add(new GitHubFileMetadata
                        {
                            Name = el.GetProperty("name").GetString() ?? "",
                            Sha = el.GetProperty("sha").GetString() ?? "",
                            DownloadUrl = el.TryGetProperty("download_url", out var dlUrl) ? dlUrl.GetString() : null
                        });
                    }
                }
            }

            return list;
        }

        private class GitHubFileMetadata
        {
            public string Name { get; set; } = "";
            public string Sha { get; set; } = "";
            public string? DownloadUrl { get; set; }
        }
    }
}
