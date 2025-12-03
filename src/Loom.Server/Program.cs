using Loom.Server.Configuration;
using Loom.Server.Endpoints;
using Loom.Server.Extensions;

var builder = WebApplication.CreateBuilder(args);

// 1) Load settings (strongly typed)
builder.Services.Configure<ServerSettings>(builder.Configuration.GetSection("Loom"));

// 2) Add infrastructure and application modules
builder.Services.AddLoomServices(builder.Configuration);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = null;
});

// 3) Add Swagger / API docs
builder.Services.AddSwaggerDocumentation();

// 4) Add health checks (for Docker / uptime)
builder.Services.AddServerHealthChecks();

// 5) Register API Key middleware HERE
builder.Services.AddApiKeyAuth();

// 6) Adding Cors
builder.Services.AddLoomCors();

var app = builder.Build();

// 7) Swagger
app.UseSwaggerDocumentation();

// 8) API Key auth middleware
app.UseApiKeyAuth();

// 9) Routes
app.MapTaskEndpoints();
app.MapHealthChecks("/health");

app.Run("http://0.0.0.0:5184");
