using Loom.Application.Interfaces;
using Loom.Core.Entities;

namespace Loom.Infrastructure.Registry;

public class CommandRegistry : ICommandRegistry
{
    private readonly Dictionary<string, CommandDefinition> _commands = new(
        StringComparer.OrdinalIgnoreCase
    );

    public void Register(CommandDefinition command)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        _commands[command.Id] = command;
    }

    public IEnumerable<CommandDefinition> GetAll() => _commands.Values;

    public CommandDefinition? GetById(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;

        return _commands.TryGetValue(id, out var command) ? command : null;
    }

    public bool CanExecute(string id)
    {
        var command = GetById(id);
        return command?.IsEnabled ?? false;
    }

    public void Execute(string id)
    {
        if (!_commands.TryGetValue(id, out var command))
            return;

        if (!command.IsEnabled)
            return;

        try
        {
            command.Action?.Invoke();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[Command Error] {command.Id}: {ex.Message}");
            Console.Error.WriteLine(ex.StackTrace);

            throw;
        }
    }
}
