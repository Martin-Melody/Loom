using Loom.Core.Entities;

namespace Loom.Application.Interfaces;

public interface ICommandRegistry
{
    void Register(CommandDefinition command);
    IEnumerable<CommandDefinition> GetAll();
    CommandDefinition? GetById(string id);
    bool CanExecute(string id);
    void Execute(string id);
}
