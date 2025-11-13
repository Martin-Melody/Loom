using Loom.Application.DTOs.Tasks;
using Loom.Application.Interfaces;
using Loom.Core.Entities;

namespace Loom.Application.Services;

public sealed class TaskService : ITaskService
{
    private readonly ITaskRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly IDateTimeProvider _clock;

    public TaskService(ITaskRepository repo, IUnitOfWork uow, IDateTimeProvider clock)
    {
        _repo = repo;
        _uow = uow;
        _clock = clock;
    }

    public async Task<IReadOnlyList<TaskView>> GetTasksAsync(
        TaskFilter? filter = null,
        CancellationToken ct = default
    )
    {
        var items = await _repo.ListAsync(ct);

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

    public async Task<TaskView> AddTaskAsync(AddTaskRequest req, CancellationToken ct = default)
    {
        var entity = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = req.Title,
            Notes = req.Notes,
            DueDate = req.Due,
            Status = TaskItemStatus.Pending,
            CreatedAt = _clock.UtcNow,
        };

        await _repo.AddAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);
        return new TaskView(entity);
    }

    public async Task<TaskView> UpdateTaskAsync(EditTaskRequest req, CancellationToken ct = default)
    {
        var entity =
            await _repo.GetByIdAsync(req.Id, ct)
            ?? throw new InvalidOperationException("Task not found.");

        entity.Title = req.Title ?? "";
        entity.Notes = req.Notes;
        entity.DueDate = req.Due;
        entity.UpdatedAt = _clock.UtcNow;

        await _repo.UpdateAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);
        return new TaskView(entity);
    }

    public async Task DeleteTaskAsync(Guid id, CancellationToken ct = default)
    {
        await _repo.DeleteAsync(id, ct);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task ToggleCompleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(id, ct);
        if (entity is null)
            return;

        entity.ToggleComplete();
        entity.UpdatedAt = _clock.UtcNow;

        await _repo.UpdateAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);
    }
}
