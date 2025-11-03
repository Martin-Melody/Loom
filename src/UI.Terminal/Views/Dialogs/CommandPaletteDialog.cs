using Loom.Application.Interfaces;
using Terminal.Gui;
using LoomCommand = Loom.Core.Entities.Command;
using TuiApp = Terminal.Gui.Application;

namespace Loom.UI.Terminal.Views.Dialogs;

public sealed class CommandPaletteDialog : Dialog
{
    private readonly TextField _searchBox = null!;
    private readonly ListView _listView = null!;
    private readonly List<LoomCommand> _filtered;
    private readonly ICommandRegistry _registry;

    public CommandPaletteDialog(ICommandRegistry registry)
    {
        _registry = registry;
        _filtered = _registry.GetAll().ToList();

        Title = "Command Palette";
        Width = 60;
        Height = 20;

        // --- Search box ---
        _searchBox = new TextField("")
        {
            X = 1,
            Y = 1,
            Width = Dim.Fill(1),
        };
        _searchBox.TextChanged += OnSearchChanged;

        // --- Command list ---
        _listView = new ListView(_filtered)
        {
            X = 1,
            Y = Pos.Bottom(_searchBox) + 1,
            Width = Dim.Fill(1),
            Height = Dim.Fill(2),
        };

        Add(_searchBox, _listView);

        // --- Buttons ---
        var runButton = new Button("Run") { IsDefault = true };
        runButton.Clicked += async (_, __) =>
        {
            if (_listView.SelectedItem < 0)
                return;

            var command = _filtered[_listView.SelectedItem];
            TuiApp.RequestStop();
            await command.Action();
        };

        var cancelButton = new Button("Cancel");
        cancelButton.Clicked += (_, __) => TuiApp.RequestStop();

        AddButton(runButton);
        AddButton(cancelButton);
    }

    private void OnSearchChanged(object? sender, TextChangedEventArgs e)
    {
        var text = _searchBox.Text?.ToString() ?? string.Empty;
        _filtered.Clear();

        foreach (
            var c in _registry
                .GetAll()
                .Where(c =>
                    c.Name.Contains(text, StringComparison.OrdinalIgnoreCase)
                    || c.Category.Contains(text, StringComparison.OrdinalIgnoreCase)
                )
        )
        {
            _filtered.Add(c);
        }

        _listView.SetSource(_filtered);
    }
}
