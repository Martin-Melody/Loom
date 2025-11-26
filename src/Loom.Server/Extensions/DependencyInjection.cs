using Loom.Application.Interfaces;
using Loom.Infrastructure.Persistence.Json;
using Loom.Infrastructure.Persistence.Providers;
using Loom.Infrastructure.Time;
using Loom.Server.Configuration;
using Microsoft.Extensions.Options;

namespace Loom.Server.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddLoomServices(
        this IServiceCollection services,
        IConfiguration config
    )
    {
        // Load strongly-typed settings
        services.Configure<ServerSettings>(config.GetSection("Loom"));

        // Resolve settings via DI
        services.AddSingleton(provider =>
        {
            var settings = provider.GetRequiredService<IOptions<ServerSettings>>().Value;
            return settings.DataDirectory;
        });

        // Infrastructure
        services.AddSingleton<ITaskRepository>(provider =>
        {
            var dataDir = provider.GetRequiredService<string>();
            return new JsonTaskRepository(dataDir);
        });

        services.AddSingleton<IUnitOfWork, JsonUnitOfWork>();
        services.AddSingleton<IDateTimeProvider, SystemClock>();

        // Storage Provider (server is ALWAYS local)
        services.AddSingleton<IStorageProvider, LocalStorageProvider>();

        // Application services
        services.AddSingleton<ITaskService, TaskService>();

        return services;
    }
}
