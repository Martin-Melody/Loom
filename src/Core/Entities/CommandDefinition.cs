namespace Loom.Core.Entities;

public class CommandDefinition
{
    public string Id { get; }
    public string Name { get; }
    public string Category { get; }
    public string? Description { get; }
    public string? Shortcut { get; }
    public bool IsGlobalShortcut { get; }
    public Action Action { get; }
    public Func<bool>? CanExecute { get; }
    public bool IsEnabled => CanExecute?.Invoke() ?? true;

    public CommandDefinition(
        string id,
        string name,
        string category,
        Action action,
        string? description = null,
        string? shortcut = null,
        bool isGlobalShortcut = false,
        Func<bool>? canExecute = null
    )
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Command id cannot be null or whitespace.", nameof(id));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Command name cannot be null or whitespace.", nameof(name));
        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException(
                "Command category cannot be null or whitespace.",
                nameof(category)
            );
        Action = action ?? throw new ArgumentNullException(nameof(action));

        Id = id;
        Name = name;
        Category = category;
        Description = description;
        Shortcut = shortcut;
        IsGlobalShortcut = isGlobalShortcut;
        CanExecute = canExecute;
    }

    public static Func<Task> ToAsync(Action action)
    {
        if (action is null)
            throw new ArgumentException(nameof(action));

        return () =>
        {
            action();
            return Task.CompletedTask;
        };
    }

    public override string ToString()
    {
        var shortcutSuffix = string.IsNullOrWhiteSpace(Shortcut) ? string.Empty : $" ({Shortcut})";
        return $"{Category}: {Name}{shortcutSuffix}";
    }
}
