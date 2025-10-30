using Loom.Application.Interfaces;

namespace Loom.Infrastructure.Persistence.Json;

public sealed class JsonUnitOfWork : IUnitOfWork
{
    private readonly JsonTaskRepository _tasks;

    public JsonUnitOfWork(JsonTaskRepository tasks)
    {
        _tasks = tasks;
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _tasks.CommitAsync(ct);
        // When you add Habit/Project repos, commit them here too.
    }
}

