using Loom.Application.UseCases.Tasks;
using Terminal.Gui;
using TuiApp = Terminal.Gui.Application;

namespace Loom.UI.Terminal.Views.Dialogs;

public class AddTaskDialog : Dialog
{
    private readonly AddTask _addTask;

    private readonly TextField _titleField;
    private readonly Button _okButton;
    private readonly Button _cancelButton;

    public bool TaskCreated { get; private set; } = false;

    public AddTaskDialog(AddTask addTask)
        : base()
    {
        _addTask = addTask;

        var lblTitle = new Label("Title:") { X = 1, Y = 1 };
        _titleField = new TextField
        {
            X = 10,
            Y = 1,
            Width = Dim.Fill() - 2,
        };

        _okButton = new Button("OK")
        {
            IsDefault = true,
            X = Pos.Center() - 10,
            Y = Pos.Bottom(_titleField) + 2,
        };

        _cancelButton = new Button("Cancel")
        {
            X = Pos.Center() + 2,
            Y = Pos.Bottom(_titleField) + 2,
        };

        _okButton.Clicked += async (_, __) =>
        {
            var title = _titleField.Text.ToString() ?? "";
            if (!string.IsNullOrWhiteSpace(title))
            {
                await _addTask.Handle(title, null, DateOnly.FromDateTime(DateTime.UtcNow));
                TaskCreated = true;
                TuiApp.RequestStop(this);
            }
        };

        _cancelButton.Clicked += (_, __) => TuiApp.RequestStop(this);

        Add(lblTitle, _titleField, _okButton, _cancelButton);
    }
}
