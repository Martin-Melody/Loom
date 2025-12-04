namespace Loom.Application.DTOs;

public sealed class ConnectionTestResult
{
    public bool Success { get; init; }
    public string? Error { get; init; }
    public string? ServerVersion { get; init; }
}
