using System.Net.Http.Json;
using System.Text.Json;
using Loom.Application.DTOs;
using Loom.Application.DTOs.Tasks;
using Loom.Application.Interfaces;
using Loom.Core.Entities;

namespace Loom.Infrastructure.Persistence.Providers;

public class RemoteApiProvider : IStorageProvider, IConnectionTester
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
            // Make sure only one header exists
            _http.DefaultRequestHeaders.Remove("X-API-Key");
            _http.DefaultRequestHeaders.Add("X-API-Key", apiKey);
        }
    }

    public async Task<ConnectionTestResult> TestConnectionAsync(CancellationToken ct = default)
    {
        var url = $"{_serverUrl}/health";

        try
        {
            var response = await _http.GetAsync(url, ct);
            if (!response.IsSuccessStatusCode)
            {
                return new ConnectionTestResult
                {
                    Success = false,
                    Error = $"Server returned {response.StatusCode}",
                };
            }

            return new ConnectionTestResult { Success = true };
        }
        catch (HttpRequestException ex)
        {
            return new ConnectionTestResult
            {
                Success = false,
                Error = "Network error: " + ex.Message,
            };
        }
        catch (TaskCanceledException)
        {
            return new ConnectionTestResult { Success = false, Error = "Request timed out" };
        }
        catch (Exception ex)
        {
            return new ConnectionTestResult
            {
                Success = false,
                Error = "Unexpected error: " + ex.Message,
            };
        }
    }

    public async Task<IReadOnlyList<TaskItem>> GetTasksAsync(CancellationToken ct = default)
    {
        var url = $"{_serverUrl}/tasks";
        return await _http.GetFromJsonAsync<IReadOnlyList<TaskItem>>(url, _jsonOptions, ct)
            ?? Array.Empty<TaskItem>();
    }

    public async Task<TaskItem> AddTaskAsync(AddTaskRequest request, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync($"{_serverUrl}/tasks", request, ct);
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
        var response = await _http.PutAsJsonAsync($"{_serverUrl}/tasks/{request.Id}", request, ct);
        response.EnsureSuccessStatusCode();

        return (
            await response.Content.ReadFromJsonAsync<TaskItem>(_jsonOptions, cancellationToken: ct)
        )!;
    }

    public async Task<TaskItem> ToggleCompleteAsync(Guid id, CancellationToken ct = default)
    {
        var response = await _http.PostAsync($"{_serverUrl}/tasks/{id}/toggle", null, ct);
        response.EnsureSuccessStatusCode();

        return (
            await response.Content.ReadFromJsonAsync<TaskItem>(_jsonOptions, cancellationToken: ct)
        )!;
    }

    public async Task DeleteTaskAsync(Guid id, CancellationToken ct = default)
    {
        var response = await _http.DeleteAsync($"{_serverUrl}/tasks/{id}", ct);
        response.EnsureSuccessStatusCode();
    }
}
