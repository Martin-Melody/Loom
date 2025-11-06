using Loom.Application.Interfaces;
using Loom.Core.Entities;

namespace Loom.Application.UseCases.Tasks;

public sealed class AddTask
{
    private readonly ITaskRepository _repo;
    private readonly IUnitOfWork _uow;

    public AddTask(ITaskRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<TaskItem> Handle(
        string title,
        string? notes,
        DateOnly? due,
        CancellationToken ct = default
    )
    {
        var task = new TaskItem
        {
            Title = title,
            Notes = notes,
            DueDate = due,
            UpdatedAt = DateTime.UtcNow,
        };
        await _repo.AddAsync(task, ct);
        await _uow.SaveChangesAsync(ct);
        return task;
    }
}
