using Loom.Application.Interfaces;
using Loom.Application.Services;
using Loom.Infrastructure.Persistence.Json;
using Loom.Infrastructure.Time;

namespace Loom.CLI;

internal static class CliServices
{
    public static ITaskService CreateTaskService()
    {
        var dataDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".loom"
        );

        var repo = new JsonTaskRepository(dataDir);
        IUnitOfWork uow = new JsonUnitOfWork(repo);
        IDateTimeProvider clock = new SystemClock();

        return new TaskService(repo, uow, clock);
    }
}
