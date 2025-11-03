using Loom.Core.Entities;

namespace Loom.Application.Interfaces;

public interface IConfigRepository
{
    Task<AppConfig> LoadAsync(CancellationToken ct = default);

    Task SaveAsync(AppConfig config, CancellationToken ct = default);
}
