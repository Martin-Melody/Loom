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
        : base()
    {
        _commands = commands;

        X = 0;
        Y = 1;
        Width = 25;
        Height = Dim.Fill();
        BorderStyle = LineStyle.Single;
        CanFocus = true;

        Border.Thickness = new Thickness(left: 0, top: 0, right: 1, bottom: 0);

        _listView = new ListView(new List<string>())
        {
            X = 1,
            Y = 1,
            Width = Dim.Fill(),
            // Height = Dim.Fill(),
            CanFocus = true,
            BorderStyle = LineStyle.Single,
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

        var items = _navCommands.Select(FormatLabel).ToList();
        _listView.SetSource(items);

        _listView.Height = Dim.Function(() =>
        {
            var itemCount = _navCommands?.Count ?? 0;
            return Math.Min(itemCount + 2, Bounds.Height);
        });

        _listView.Title = "Navigation";

        _listView.BorderStyle = LineStyle.Single;

        var bordr = new Thickness(top: 1, bottom: 1, right: 0, left: 0);

        _listView.Border.Thickness = bordr;
        SetNeedsDisplay();
    }

    public void FocusList() => _listView.SetFocus();

    private static string FormatLabel(CommandDefinition cmd)
    {
        return cmd.Name;
    }
}
