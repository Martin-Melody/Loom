using Loom.Application.Interfaces;
using Loom.Core.Entities;
using Loom.Core.Entities.Enums;

namespace Loom.Application.Services;

public class AppStateService
{
    private readonly IConfigRepository _configRepo;
    private AppConfig _config = new();

    public AppStateService(IConfigRepository configRepo)
    {
        _configRepo = configRepo;
    }

    public async Task InitalizeAsync(CancellationToken ct = default)
    {
        _config = await _configRepo.LoadAsync(ct);
    }

    public ViewType LastOpenView
    {
        get => _config.LastOpenView;
        set => _config.LastOpenView = value;
    }

    public SidebarState SidebarState
    {
        get => _config.SidebarState;
        set => _config.SidebarState = value;
    }

    public async Task SaveAsync(CancellationToken ct = default)
    {
        await _configRepo.SaveAsync(_config, ct);
    }
}
