using System.Net.Http.Json;
using System.Text.Json;
using Loom.Application.DTOs.Tasks;
using Loom.Application.Interfaces;
using Loom.Core.Entities;

namespace Loom.Infrastructure.Persistence.Providers;

public class RemoteApiProvider : IStorageProvider
{
    private readonly HttpClient _http;
    private readonly string _serverUrl;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public RemoteApiProvider(HttpClient http, string serverUrl, string? apiKey)
    {
        _http = http;
        _serverUrl = serverUrl.TrimEnd('/');

        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            _http.DefaultRequestHeaders.Remove("X-API-Key");
            _http.DefaultRequestHeaders.Add("X-API-Key", apiKey);
        }
    }

    public async Task<IReadOnlyList<TaskItem>> GetTasksAsync(CancellationToken ct = default)
    {
        var url = $"{_serverUrl}/tasks";
        var tasks = await _http.GetFromJsonAsync<IReadOnlyList<TaskItem>>(url, _jsonOptions, ct);

        if (tasks is null)
            return Array.Empty<TaskItem>();

        return tasks;
    }

    public async Task<TaskItem> AddTaskAsync(AddTaskRequest request, CancellationToken ct = default)
    {
        var url = $"{_serverUrl}/tasks";

        var response = await _http.PostAsJsonAsync(url, request, ct);
        response.EnsureSuccessStatusCode();

        return (
            await response.Content.ReadFromJsonAsync<TaskItem>(_jsonOptions, cancellationToken: ct)
        )!;
    }

    public async Task<TaskItem> UpdateTaskAsync(
        EditTaskRequest request,
        CancellationToken ct = default
    )
    {
        var url = $"{_serverUrl}/tasks/{request.Id}";

        var response = await _http.PutAsJsonAsync(url, request, ct);
        response.EnsureSuccessStatusCode();

        return (
            await response.Content.ReadFromJsonAsync<TaskItem>(_jsonOptions, cancellationToken: ct)
        )!;
    }

    public async Task<TaskItem> ToggleCompleteAsync(Guid id, CancellationToken ct = default)
    {
        var url = $"{_serverUrl}/tasks/{id}/toggle";

        var response = await _http.PostAsync(url, null, ct);
        response.EnsureSuccessStatusCode();

        return (
            await response.Content.ReadFromJsonAsync<TaskItem>(_jsonOptions, cancellationToken: ct)
        )!;
    }

    public async Task DeleteTaskAsync(Guid id, CancellationToken ct = default)
    {
        var url = $"{_serverUrl}/tasks/{id}";

        var response = await _http.DeleteAsync(url, ct);
        response.EnsureSuccessStatusCode();
    }
}
