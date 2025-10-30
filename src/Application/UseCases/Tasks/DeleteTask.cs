using Loom.Application.Interfaces;

namespace Loom.Application.UseCases.Tasks;

public sealed class DeleteTask
{
    private readonly ITaskRepository _repo;
    private readonly IUnitOfWork _uow;

    public DeleteTask(ITaskRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task Handle(Guid id, CancellationToken ct = default)
    {
        await _repo.DeleteAsync(id, ct);
        await _uow.SaveChangesAsync(ct);
    }
}
