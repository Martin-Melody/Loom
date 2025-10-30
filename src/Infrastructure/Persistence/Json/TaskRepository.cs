using Loom.Application.Interfaces;
using Loom.Core.Entities;

namespace Loom.Infrastructure.Persistence.Json;

public sealed class JsonTaskRepository : ITaskRepository
{
    private readonly JsonStore<TaskItem> _store;
    private List<TaskItem>? _cache;

    public JsonTaskRepository(string baseDir)
    {
        _store = new JsonStore<TaskItem>(baseDir, "tasks.json");
    }

    private async Task<List<TaskItem>> Load(CancellationToken ct)
        => _cache ??= await _store.LoadAsync(ct);

    public async Task AddAsync(TaskItem entity, CancellationToken ct = default)
    {
        var data = await Load(ct);
        data.Add(entity);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var data = await Load(ct);
        data.RemoveAll(t => t.Id == id);
    }

    public async Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => (await Load(ct)).FirstOrDefault(t => t.Id == id);

    public async Task<IReadOnlyList<TaskItem>> ListAsync(CancellationToken ct = default)
        => await Load(ct);

    public async Task UpdateAsync(TaskItem entity, CancellationToken ct = default)
    {
        var data = await Load(ct);
        var idx = data.FindIndex(t => t.Id == entity.Id);
        if (idx >= 0) data[idx] = entity;
    }

    public async Task<IReadOnlyList<TaskItem>> ListPendingAsync(CancellationToken ct = default)
        => (await Load(ct)).Where(t => t.Status == TaskItemStatus.Pending).ToList();

    public async Task<IReadOnlyList<TaskItem>> ListDueOnAsync(DateOnly date, CancellationToken ct = default)
        => (await Load(ct)).Where(t => t.DueDate == date).ToList();

    // Commit is handled by UnitOfWork
    internal async Task CommitAsync(CancellationToken ct) => await _store.SaveAsync(await Load(ct), ct);
}

