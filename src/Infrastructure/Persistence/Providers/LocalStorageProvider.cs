using Loom.Application.DTOs.Tasks;
using Loom.Application.Interfaces;
using Loom.Core.Entities;

namespace Loom.Infrastructure.Persistence.Providers;

public class LocalStorageProvider : IStorageProvider
{
    private readonly ITaskRepository _tasks;
    private readonly IUnitOfWork _uow;

    public LocalStorageProvider(ITaskRepository tasks, IUnitOfWork uow)
    {
        _tasks = tasks;
        _uow = uow;
    }

    public Task<IReadOnlyList<TaskItem>> GetTasksAsync(CancellationToken ct = default)
    {
        return _tasks.ListAsync(ct);
    }

    public async Task<TaskItem> AddTaskAsync(AddTaskRequest request, CancellationToken ct = default)
    {
        var item = new TaskItem
        {
            Title = request.Title,
            Notes = request.Notes,
            DueDate = request.Due,
            CreatedAt = DateTime.UtcNow,
        };

        await _tasks.AddAsync(item, ct);
        await _uow.SaveChangesAsync(ct);

        return item;
    }

    public async Task<TaskItem> UpdateTaskAsync(
        EditTaskRequest request,
        CancellationToken ct = default
    )
    {
        var item =
            await _tasks.GetByIdAsync(request.Id, ct) ?? throw new Exception("Task not found.");

        item.Title = request.Title ?? item.Title;
        item.Notes = request.Notes ?? item.Notes;
        item.DueDate = request.Due ?? item.DueDate;
        item.UpdatedAt = DateTime.UtcNow;

        await _tasks.UpdateAsync(item, ct);
        await _uow.SaveChangesAsync(ct);

        return item;
    }

    public async Task<TaskItem> ToggleCompleteAsync(Guid id, CancellationToken ct = default)
    {
        var item = await _tasks.GetByIdAsync(id, ct) ?? throw new Exception("Task not found.");

        item.ToggleComplete();
        item.UpdatedAt = DateTime.UtcNow;

        await _tasks.UpdateAsync(item, ct);
        await _uow.SaveChangesAsync(ct);

        return item;
    }

    public async Task DeleteTaskAsync(Guid id, CancellationToken ct = default)
    {
        await _tasks.DeleteAsync(id, ct);
        await _uow.SaveChangesAsync(ct);
    }
}
