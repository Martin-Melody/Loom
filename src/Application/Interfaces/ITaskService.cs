using Loom.Application.DTOs.Tasks;

namespace Loom.Application.Interfaces;

public interface ITaskService
{
    Task<IReadOnlyList<TaskView>> GetTasksAsync(
        TaskFilter? filter = null,
        CancellationToken ct = default
    );
    Task<TaskView> AddTaskAsync(AddTaskRequest request, CancellationToken ct = default);
    Task<TaskView> UpdateTaskAsync(EditTaskRequest request, CancellationToken ct = default);
    Task DeleteTaskAsync(Guid id, CancellationToken ct = default);
    Task ToggleCompleteAsync(Guid id, CancellationToken ct = default);
}
