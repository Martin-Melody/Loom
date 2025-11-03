using Loom.Application.Interfaces;
using Loom.Core.Entities;

namespace Loom.Infrastructure.Services;

public class CommandRegistry : ICommandRegistry
{
    private readonly List<Command> _commands = new();

    public void Register(Command command) => _commands.Add(command);

    public IEnumerable<Command> GetAll() => _commands;
}
