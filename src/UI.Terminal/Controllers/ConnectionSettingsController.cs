using Loom.Application.Services;
using Loom.Core.Entities.Enums;
using Loom.Infrastructure.Persistence.Providers;
using Loom.UI.Terminal.Views.Dialogs;
using TuiApp = Terminal.Gui.Application;

namespace Loom.UI.Terminal.Controllers;

public sealed class ConnectionSettingsController
{
    private readonly AppStateService _appState;
    private readonly Action<bool, string>? _updateStatusBar;

    public ConnectionSettingsController(
        AppStateService appState,
        Action<bool, string>? updateStatusBar = null
    )
    {
        _appState = appState;
        _updateStatusBar = updateStatusBar;
    }

    // Entry point - identical to CommandPaletteController.Show()
    public void Show()
    {
        var dialog = new ConnectionSettingsDialog();

        // Preload values
        dialog.ModeRadio.SelectedItem = _appState.ConnectionMode == ConnectionMode.Remote ? 1 : 0;

        dialog.UrlField.Text = _appState.ServerUrl ?? "";
        dialog.ApiKeyField.Text = _appState.ApiKey ?? "";

        // Wire button handlers
        dialog.TestButton.Clicked += async (_, __) => await Test(dialog);
        dialog.SaveButton.Clicked += async (_, __) => await Save(dialog);
        dialog.CloseButton.Clicked += (_, __) => TuiApp.RequestStop(dialog);

        TuiApp.Run(dialog); // Show modal dialog
    }

    private async Task Test(ConnectionSettingsDialog dialog)
    {
        var mode =
            dialog.ModeRadio.SelectedItem == 1 ? ConnectionMode.Remote : ConnectionMode.Local;

        if (mode == ConnectionMode.Local)
        {
            dialog.StatusLabel.Text = "Local mode: nothing to test.";
            _updateStatusBar?.Invoke(true, "Local");
            return;
        }

        var url = dialog.UrlField.Text.ToString() ?? "";
        var apiKey = dialog.ApiKeyField.Text.ToString() ?? "";

        if (string.IsNullOrWhiteSpace(url))
        {
            dialog.StatusLabel.Text = "Please enter a server URL.";
            return;
        }

        dialog.StatusLabel.Text = "Testing connection...";

        using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
        var provider = new RemoteApiProvider(http, url, apiKey);
        var result = await provider.TestConnectionAsync();

        if (result.Success)
        {
            dialog.StatusLabel.Text = "✓ Connected successfully";
            _updateStatusBar?.Invoke(true, "Remote");
        }
        else
        {
            dialog.StatusLabel.Text = $"✗ Failed: {result.Error}";
            _updateStatusBar?.Invoke(false, "Remote");
        }
    }

    private async Task Save(ConnectionSettingsDialog dialog)
    {
        _appState.ConnectionMode =
            dialog.ModeRadio.SelectedItem == 1 ? ConnectionMode.Remote : ConnectionMode.Local;

        _appState.ServerUrl = dialog.UrlField.Text.ToString();
        _appState.ApiKey = dialog.ApiKeyField.Text.ToString();

        await _appState.SaveAsync();

        dialog.StatusLabel.Text = "Settings saved ✔";
    }
}
