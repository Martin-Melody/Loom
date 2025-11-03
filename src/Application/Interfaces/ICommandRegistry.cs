using Loom.Core.Entities;

namespace Loom.Application.Interfaces;

public interface ICommandRegistry
{
    void Register(Command command);
    IEnumerable<Command> GetAll();
}
