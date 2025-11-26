using Loom.Application.Interfaces;
using Loom.Application.Services;
using Loom.Core.Entities.Enums;
using Loom.Infrastructure.Persistence;
using Loom.Infrastructure.Persistence.Json;
using Loom.Infrastructure.Persistence.Providers;
using Loom.Infrastructure.Time;

namespace Loom.CLI;

public static class CliServices
{
    public static async Task<ITaskService> CreateTaskServiceAsync()
    {
        // --- Load config ---
        var configRepo = new ConfigRepository();
        var appState = new AppStateService(configRepo);
        await appState.InitalizeAsync();
        var config = appState.Current;

        // Determine data directory
        var dataDir = string.IsNullOrWhiteSpace(config.DataDirectory)
            ? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".loom"
            )
            : config.DataDirectory;

        IStorageProvider storageProvider;
        IDateTimeProvider clock = new SystemClock();

        // --- Choose provider based on enum ---
        switch (config.Mode)
        {
            case ConnectionMode.Remote:
                if (string.IsNullOrWhiteSpace(config.ServerUrl))
                    throw new InvalidOperationException(
                        "ConnectionMode.Remote requires a ServerUrl in config.json"
                    );

                var http = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
                storageProvider = new RemoteApiProvider(http, config.ServerUrl!, config.ApiKey);
                break;

            case ConnectionMode.Local:
            default:
                var tasksRepo = new JsonTaskRepository(dataDir);
                var uow = new JsonUnitOfWork(tasksRepo);
                storageProvider = new LocalStorageProvider(tasksRepo, uow);
                break;
        }

        return new TaskService(storageProvider, clock);
    }
}
