using Loom.Application.DTOs;

namespace Loom.Application.Interfaces;

public interface IConnectionTester
{
    Task<ConnectionTestResult> TestConnectionAsync(CancellationToken ct = default);
}
