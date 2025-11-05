using Loom.Core.Entities;

namespace Loom.Application.Interfaces;

/// <summary>
/// Implemented by each module (Task, Dashboard etc)
/// to provide its available commands
/// </summary>
public interface ICommandProvider
{
    IEnumerable<Command> GetCommands();
}
