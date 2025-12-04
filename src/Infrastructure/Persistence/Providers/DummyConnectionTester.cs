using Loom.Application.DTOs;
using Loom.Application.Interfaces;

namespace Loom.Infrastructure.Persistence.Providers;

public sealed class DummyConnectionTester : IConnectionTester
{
    public Task<ConnectionTestResult> TestConnectionAsync(CancellationToken ct = default)
    {
        return Task.FromResult(new ConnectionTestResult { Success = true, Error = null });
    }
}
