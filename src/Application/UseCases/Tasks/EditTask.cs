using Loom.Application.Interfaces;
using Loom.Core.Entities;

namespace Loom.Application.UseCases.Tasks;

public sealed class EditTask
{
    private readonly ITaskRepository _repo;
    private readonly IUnitOfWork _uow;

    public EditTask(ITaskRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<TaskItem?> Handle(Guid id, string? title = null, string? notes = null, DateOnly? due = null, CancellationToken ct = default)
    {
        var task = await _repo.GetByIdAsync(id, ct);
        if (task is null) return null;

        if (!string.IsNullOrWhiteSpace(title))
            task.Title = title;

        if (notes is not null)
            task.Notes = notes;

        if (due is not null)
            task.DueDate = due;

        task.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(task);
        await _uow.SaveChangesAsync(ct);

        return task;
    }
}

