using Loom.Server.Configuration;
using Microsoft.Extensions.Options;

namespace Loom.Server.Middleware;

public sealed class ApiKeyMiddleware : IMiddleware
{
    private const string HeaderName = "X-API-Key";
    private readonly string _expectedApiKey;

    public ApiKeyMiddleware(IOptions<ServerSettings> settings)
    {
        _expectedApiKey = settings.Value.ApiKey;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // Allow health checks without auth
        if (context.Request.Path.StartsWithSegments("/health"))
        {
            await next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(HeaderName, out var providedKey))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(
                new { error = "Missing API key", header = HeaderName }
            );
            return;
        }

        if (!string.Equals(providedKey, _expectedApiKey, StringComparison.Ordinal))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { error = "Invalid API key" });
            return;
        }

        await next(context);
    }
}
