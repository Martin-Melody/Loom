using Loom.Server.Middleware;

namespace Loom.Server.Extensions;

public static class AuthExtensions
{
    public static IServiceCollection AddApiKeyAuth(this IServiceCollection services)
    {
        services.AddSingleton<ApiKeyMiddleware>();
        return services;
    }

    public static IApplicationBuilder UseApiKeyAuth(this IApplicationBuilder app)
    {
        app.UseMiddleware<ApiKeyMiddleware>();
        return app;
    }
}
