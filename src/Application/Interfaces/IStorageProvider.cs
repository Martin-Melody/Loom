using Loom.Application.DTOs.Tasks;
using Loom.Core.Entities;

namespace Loom.Application.Interfaces;

public interface IStorageProvider
{
    Task<IReadOnlyList<TaskItem>> GetTasksAsync(CancellationToken ct = default);

    Task<TaskItem> AddTaskAsync(AddTaskRequest request, CancellationToken ct = default);

    Task<TaskItem> UpdateTaskAsync(EditTaskRequest request, CancellationToken ct = default);

    Task<TaskItem> ToggleCompleteAsync(Guid id, CancellationToken ct = default);

    Task DeleteTaskAsync(Guid id, CancellationToken ct = default);
}
