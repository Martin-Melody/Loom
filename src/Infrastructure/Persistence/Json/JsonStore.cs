using System.Text.Json;

namespace Loom.Infrastructure.Persistence.Json;

internal sealed class JsonStore<T> where T : class
{
    private readonly string _path;
    private readonly JsonSerializerOptions _opts = new() { WriteIndented = true };

    public JsonStore(string baseDir, string fileName)
    {
        Directory.CreateDirectory(baseDir);
        _path = Path.Combine(baseDir, fileName);
    }

    public async Task<List<T>> LoadAsync(CancellationToken ct = default)
    {
        if (!File.Exists(_path)) return new();
        await using var fs = File.OpenRead(_path);
        var list = await JsonSerializer.DeserializeAsync<List<T>>(fs, cancellationToken: ct) ?? new();
        return list;
    }

    public async Task SaveAsync(IEnumerable<T> items, CancellationToken ct = default)
    {
        await using var fs = File.Create(_path);
        await JsonSerializer.SerializeAsync(fs, items, _opts, ct);
    }
}
