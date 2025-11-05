namespace Loom.Core.Entities;

public class Command
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? Shortcut { get; set; }
    public Func<Task> Action { get; set; }

    public Command(
        Guid id,
        string name,
        string category,
        Func<Task> action,
        string? shortcut = null
    )
    {
        Id = id;
        Name = name;
        Category = category;
        Action = action;
        Shortcut = shortcut;
    }

    public override string ToString() => $"{Category}: {Name}";
}
