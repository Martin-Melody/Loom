namespace Loom.Server.Extensions;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddServerHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks();
        return services;
    }
}
