namespace Loom.Server.Extensions;

public static class CorsExtensions
{
    //TODO: make these more restrictive later
    private const string CorsPolicyName = "_corsPolicy";

    public static IServiceCollection AddLoomCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(
                name: CorsPolicyName,
                policy =>
                {
                    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                }
            );
        });

        return services;
    }

    public static IApplicationBuilder UseLoomCors(this IApplicationBuilder app)
    {
        app.UseCors(CorsPolicyName);
        return app;
    }
}
