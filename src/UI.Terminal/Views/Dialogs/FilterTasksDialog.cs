using Loom.Application.DTOs.Tasks;
using Terminal.Gui;
using TuiApp = Terminal.Gui.Application;

namespace Loom.UI.Terminal.Views.Dialogs;

public class FilterTasksDialog : BaseDialog
{
    private readonly RadioGroup _statusGroup;
    private readonly TextField _dueBefore;
    private readonly TextField _textSearch;
    private readonly Button _applyButton;
    private readonly Button _cancelButton;

    public TaskFilter Filter { get; private set; } = new();

    public bool Applied { get; private set; } = false;

    public FilterTasksDialog()
        : base("Filter Task", defaultHeight: 18, maxWidth: 70)
    {
        var lblStatus = new Label("Status:") { X = 1, Y = 1 };
        _statusGroup = new RadioGroup(["All", "Incomplete", "Complete"]) { X = 12, Y = 1 };

        var lblDue = new Label("Due before:") { X = 1, Y = 4 };
        _dueBefore = new TextField("")
        {
            X = 12,
            Y = 4,
            Width = 15,
        };

        var lblText = new Label("Contains text:") { X = 1, Y = 6 };
        _textSearch = new TextField("")
        {
            X = 12,
            Y = 6,
            Width = 30,
        };

        _applyButton = new Button("Apply")
        {
            IsDefault = true,
            X = Pos.Center() - 10,
            Y = Pos.Bottom(_textSearch) + 2,
        };
        _cancelButton = new Button("Cancel")
        {
            IsDefault = true,
            X = Pos.Center() + 2,
            Y = Pos.Bottom(_textSearch) + 2,
        };

        _applyButton.Clicked += (_, __) =>
        {
            Filter = new TaskFilter();

            switch (_statusGroup.SelectedItem)
            {
                case 1:
                    Filter.IsComplete = false;
                    break;
                case 2:
                    Filter.IsComplete = true;
                    break;
            }

            if (DateOnly.TryParse(_dueBefore.Text.ToString(), out var due))
                Filter.DueBefore = due;

            var text = _textSearch.Text.ToString()?.Trim();
            if (!string.IsNullOrEmpty(text))
                Filter.TextContains = text;

            Applied = true;
            TuiApp.RequestStop(this);
        };

        _cancelButton.Clicked += (_, __) => TuiApp.RequestStop(this);

        Add(
            lblStatus,
            _statusGroup,
            lblDue,
            _dueBefore,
            lblText,
            _textSearch,
            _applyButton,
            _cancelButton
        );
    }
}
