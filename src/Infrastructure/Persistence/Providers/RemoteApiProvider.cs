using Loom.Application.DTOs.Tasks;
using Loom.Application.Interfaces;
using Loom.Core.Entities;

namespace Loom.Infrastructure.Persistence.Providers;

public class RemoteApiProvider : IStorageProvider
{
    private readonly HttpClient _http;
    private readonly string _serverUrl;
    private readonly string? _apiKey;

    public RemoteApiProvider(HttpClient http, string serverUrl, string? apiKey)
    {
        _http = http;
        _serverUrl = serverUrl.TrimEnd('/');
        _apiKey = apiKey;

        // optional: add Authorization header now
        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            _http.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        }
    }

    public Task<IReadOnlyList<TaskItem>> GetTasksAsync(CancellationToken ct = default) =>
        throw new NotImplementedException("Remote API not implemented yet.");

    public Task<TaskItem> AddTaskAsync(AddTaskRequest request, CancellationToken ct = default) =>
        throw new NotImplementedException("Remote API not implemented yet.");

    public Task<TaskItem> UpdateTaskAsync(
        EditTaskRequest request,
        CancellationToken ct = default
    ) => throw new NotImplementedException("Remote API not implemented yet.");

    public Task<TaskItem> ToggleCompleteAsync(Guid id, CancellationToken ct = default) =>
        throw new NotImplementedException("Remote API not implemented yet.");

    public Task DeleteTaskAsync(Guid id, CancellationToken ct = default) =>
        throw new NotImplementedException("Remote API not implemented yet.");
}
