namespace Loom.Application.DTOs.Tasks;

public sealed record EditTaskRequest
{
    public Guid Id { get; init; }
    public string? Title { get; init; }
    public string? Notes { get; init; }
    public DateOnly? Due { get; init; }
}
