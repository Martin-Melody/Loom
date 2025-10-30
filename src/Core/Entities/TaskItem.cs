namespace Loom.Core.Entities;

public enum TaskItemStatus { Pending = 0, Complete = 1 }

public sealed class TaskItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = "";
    public string? Notes { get; set; }
    public DateOnly? DueDate { get; set; }
    public TaskItemStatus Status { get; set; } = TaskItemStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public void ToggleComplete()
    {
        if (Status == TaskItemStatus.Pending)
        {
            Status = TaskItemStatus.Complete;
            CompletedAt = DateTime.UtcNow;
        }
        else if (Status == TaskItemStatus.Complete)
        {
            Status = TaskItemStatus.Pending;
            CompletedAt = null;
        }

    }

}

