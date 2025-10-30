using Loom.Core.Entities;

namespace Loom.Application.Interfaces;

public interface IRepository<T>
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<T>> ListAsync(CancellationToken ct = default);
    Task AddAsync(T entity, CancellationToken ct = default);
    Task UpdateAsync(T entity, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public interface ITaskRepository : IRepository<TaskItem>
{
    Task<IReadOnlyList<TaskItem>> ListPendingAsync(CancellationToken ct = default);
    Task<IReadOnlyList<TaskItem>> ListDueOnAsync(DateOnly date, CancellationToken ct = default);
}

public interface IHabbitRepository : IRepository<Habbit> { }
public interface IProjectRepository : IRepository<Project> { }

public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken ct = default);
}

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
    DateOnly Today { get; }
}

