using Loom.Application.DTOs.Tasks;
using Loom.Application.Interfaces;
using Loom.Core.Entities;

namespace Loom.Application.UseCases.Tasks;

public sealed class FilterTasks
{
    private readonly ITaskRepository _repo;
    private readonly IDateTimeProvider _clock;

    public FilterTasks(ITaskRepository repo, IDateTimeProvider clock)
    {
        _repo = repo;
        _clock = clock;
    }

    public async Task<IReadOnlyList<TaskItem>> Handle(TaskFilter? filter = null, CancellationToken ct = default)
    {
        // Start with all tasks
        var tasks = await _repo.ListAsync(ct);

        // --- Default behavior (no filter = today's tasks) ---
        if (filter is null)
        {
            var today = _clock.Today;
            tasks = tasks.Where(t => t.DueDate == today).ToList();
            return tasks;
        }

        // --- Apply filters dynamically ---
        if (filter.IsComplete is not null)
            tasks = tasks.Where(t => (t.Status == TaskItemStatus.Complete) == filter.IsComplete).ToList();

        if (filter.DueBefore is { } dueBefore)
            tasks = tasks.Where(t => t.DueDate <= dueBefore).ToList();

        if (filter.DueOn is { } dueOn)
            tasks = tasks.Where(t => t.DueDate == dueOn).ToList();

        if (!string.IsNullOrWhiteSpace(filter.TextContains))
            tasks = tasks.Where(t =>
                t.Title.Contains(filter.TextContains, StringComparison.OrdinalIgnoreCase) ||
                (t.Notes?.Contains(filter.TextContains, StringComparison.OrdinalIgnoreCase) ?? false)
            ).ToList();

        return tasks;
    }
}

