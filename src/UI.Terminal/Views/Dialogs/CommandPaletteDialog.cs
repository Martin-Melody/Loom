using Loom.Application.Interfaces;
using Loom.Core.Entities;
using Terminal.Gui;
using TuiApp = Terminal.Gui.Application;

namespace Loom.UI.Terminal.Views.Dialogs;

public sealed class CommandPaletteDialog : BaseDialog
{
    private readonly TextField _searchBox = null!;
    private readonly ListView _listView = null!;
    private readonly List<CommandDefinition> _filtered;
    private readonly ICommandRegistry _registry;
    private CommandDefinition? _selectedCommand;

    public CommandPaletteDialog(ICommandRegistry registry)
        : base("Command Palette", defaultHeight: 18, maxWidth: 70)
    {
        _registry = registry;
        _filtered = LoadCommands(string.Empty);

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
        runButton.Clicked += (_, __) =>
        {
            if (_listView.SelectedItem < 0)
                return;

            _selectedCommand = _filtered[_listView.SelectedItem];
            TuiApp.RequestStop(this);
        };

        var cancelButton = new Button("Cancel");
        cancelButton.Clicked += (_, __) =>
        {
            _selectedCommand = null;
            TuiApp.RequestStop(this);
        };

        AddButton(runButton);
        AddButton(cancelButton);
    }

    private void OnSearchChanged(object? sender, TextChangedEventArgs e)
    {
        var text = _searchBox.Text?.ToString() ?? string.Empty;
        var commands = LoadCommands(text);

        _filtered.Clear();
        _filtered.AddRange(commands);
        _listView.SetSource(_filtered);
        _listView.SelectedItem = _filtered.Count > 0 ? 0 : -1;
    }

    private List<CommandDefinition> LoadCommands(string search)
    {
        IEnumerable<CommandDefinition> commands = _registry.GetAll().Where(c => c.IsEnabled);

        if (!string.IsNullOrWhiteSpace(search))
        {
            commands = commands.Where(c =>
                c.Name.Contains(search, StringComparison.OrdinalIgnoreCase)
                || c.Category.Contains(search, StringComparison.OrdinalIgnoreCase)
            );
        }

        return commands.OrderBy(c => c.Category).ThenBy(c => c.Name).ToList();
    }

    public CommandDefinition? SelectedCommand => _selectedCommand;
}
