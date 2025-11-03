namespace Loom.Core.Entities;

public class Command
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;

    public Func<Task> Action { get; set; }

    public Command(string name, string category, Func<Task> action)
    {
        Name = name;
        Category = category;
        Action = action;
    }

    public override string ToString() => $"{Category}: {Name}";
}
