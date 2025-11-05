using Loom.Application.Interfaces;
using Loom.Core.Entities;

namespace Loom.Infrastructure.Registry;

public class CommandRegistry
{
    private readonly List<Command> _commands = new();

    public void Register(Command command) => _commands.Add(command);

    public void RegisterProvider(ICommandProvider provider)
    {
        foreach (var cmd in provider.GetCommands())
            _commands.Add(cmd);
    }

    public IEnumerable<Command> GetAll() => _commands;
}
