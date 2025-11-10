using Loom.Application.Interfaces;
using Loom.Core.Entities;
using Terminal.Gui;

namespace Loom.UI.Terminal.Views.UI;

public class SidebarView : FrameView
{
    private readonly ListView _listView;
    private readonly ICommandRegistry _commands;
    private List<CommandDefinition> _navCommands = new();

    public SidebarView(ICommandRegistry commands)
        : base("Navigation")
    {
        _commands = commands;

        X = 0;
        Y = 1;
        Width = 25;
        Height = Dim.Fill();
        BorderStyle = LineStyle.Rounded;
        CanFocus = true;

        _listView = new ListView(new List<string>())
        {
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            CanFocus = true,
        };

        _listView.OpenSelectedItem += (_, args) =>
        {
            var cmd = _navCommands[args.Item];
            if (cmd.IsEnabled)
            {
                cmd.Action();
            }
        };

        Add(_listView);
    }

    public void LoadCommands()
    {
        _navCommands = _commands
            .GetAll()
            .Where(c => c.Category.Equals("navigation", StringComparison.OrdinalIgnoreCase))
            .OrderBy(c => c.Name)
            .ToList();

        _listView.SetSource(_navCommands.Select(n => n.Name).ToList());
        SetNeedsDisplay();
    }

    public void FocusList() => _listView.SetFocus();

    private static string FormatLabel(CommandDefinition cmd)
    {
        return cmd.Name;
    }
}
