namespace Loom.Application.DTOs.Tasks;

public class TaskFilter
{
    public bool? IsComplete { get; set; }
    public DateOnly? DueBefore { get; set; }
    public DateOnly? DueOn { get; set; }
    public string? TextContains { get; set; }
}

