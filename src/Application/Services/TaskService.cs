using Loom.Application.DTOs.Tasks;
using Loom.Application.Interfaces;
using Loom.Core.Entities;

public sealed class TaskService : ITaskService
{
    private readonly IStorageProvider _storage;
    private readonly IDateTimeProvider _clock;

    public TaskService(IStorageProvider storage, IDateTimeProvider clock)
    {
        _storage = storage;
        _clock = clock;
    }

    public async Task<IReadOnlyList<TaskView>> GetTasksAsync(
        TaskFilter? filter = null,
        CancellationToken ct = default
    )
    {
        var items = await _storage.GetTasksAsync(ct);

        if (filter is null)
        {
            var today = _clock.Today;
            items = items.Where(t => DateOnly.FromDateTime(t.CreatedAt) == today).ToList();
        }

        if (filter is not null)
        {
            items = items
                .Where(t =>
                    (
                        filter.IsComplete == null
                        || (filter.IsComplete.Value && t.Status == TaskItemStatus.Complete)
                        || (!filter.IsComplete.Value && t.Status == TaskItemStatus.Pending)
                    )
                    && (
                        filter.DueBefore == null
                        || (t.DueDate != null && t.DueDate <= filter.DueBefore)
                    )
                    && (
                        string.IsNullOrWhiteSpace(filter.TextContains)
                        || t.Title.Contains(filter.TextContains, StringComparison.OrdinalIgnoreCase)
                    )
                )
                .ToList();
        }

        return items.Select(t => new TaskView(t)).ToList();
    }

    public async Task<TaskView> AddTaskAsync(AddTaskRequest request, CancellationToken ct = default)
    {
        var item = await _storage.AddTaskAsync(request, ct);
        return new TaskView(item);
    }

    public async Task<TaskView> UpdateTaskAsync(
        EditTaskRequest request,
        CancellationToken ct = default
    )
    {
        var item = await _storage.UpdateTaskAsync(request, ct);
        return new TaskView(item);
    }

    public Task DeleteTaskAsync(Guid id, CancellationToken ct = default) =>
        _storage.DeleteTaskAsync(id, ct);

    public async Task<TaskView> ToggleCompleteAsync(Guid id, CancellationToken ct = default)
    {
        var item = await _storage.ToggleCompleteAsync(id, ct);
        return new TaskView(item);
    }
}
