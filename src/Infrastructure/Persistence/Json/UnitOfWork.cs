using Loom.Application.Interfaces;

namespace Loom.Infrastructure.Persistence.Json;

public sealed class JsonUnitOfWork : IUnitOfWork
{
    private readonly ITaskRepository _tasks;

    public JsonUnitOfWork(ITaskRepository tasks)
    {
        _tasks = tasks;
    }

    public Task SaveChangesAsync(CancellationToken ct = default)
    {
        return _tasks.CommitAsync(ct);
    }
}
