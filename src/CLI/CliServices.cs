using Loom.Application.Interfaces;
using Loom.Infrastructure.Persistence.Json;
using Loom.Infrastructure.Persistence.Providers;
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
        var storage = new LocalStorageProvider(repo, uow);
        IDateTimeProvider clock = new SystemClock();

        return new TaskService(storage, clock);
    }
}
