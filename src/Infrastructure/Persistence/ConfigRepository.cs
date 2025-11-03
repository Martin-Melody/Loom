using System.Text.Json;
using Loom.Application.Interfaces;
using Loom.Core.Entities;

namespace Loom.Infrastructure.Persistence;

public class ConfigRepository : IConfigRepository
{
    private readonly string _filePath;

    public ConfigRepository()
    {
        var configDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".config",
            "loom"
        );

        Directory.CreateDirectory(configDir);
        _filePath = Path.Combine(configDir, "config.json");
    }

    public async Task<AppConfig> LoadAsync(CancellationToken ct = default)
    {
        if (!File.Exists(_filePath))
            return new AppConfig();

        var json = await File.ReadAllTextAsync(_filePath, ct);
        return JsonSerializer.Deserialize<AppConfig>(json) ?? new AppConfig();
    }

    public async Task SaveAsync(AppConfig config, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(
            config,
            new JsonSerializerOptions { WriteIndented = true }
        );

        await File.WriteAllTextAsync(_filePath, json, ct);
    }
}
