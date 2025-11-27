namespace Loom.Application.DTOs.Tasks;

public sealed record AddTaskRequest
{
    public string Title { get; init; } = "";
    public string? Notes { get; init; }
    public DateOnly? Due { get; init; }
}
