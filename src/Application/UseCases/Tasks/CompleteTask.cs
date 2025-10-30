using Loom.Application.Interfaces;

namespace Loom.Application.UseCases.Tasks;

public sealed class ToggleCompleteTask
{
    private readonly ITaskRepository _repo;
    private readonly IUnitOfWork _uow;

    public ToggleCompleteTask(ITaskRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<bool> Handle(Guid id, CancellationToken ct = default)
    {
        var task = await _repo.GetByIdAsync(id, ct);
        if (task is null) return false;
        task.ToggleComplete();
        await _repo.UpdateAsync(task, ct);
        await _uow.SaveChangesAsync(ct);
        return true;
    }
}
