using Loom.Application.UseCases.Tasks;
using Loom.Core.Entities;
using Terminal.Gui;
using TuiApp = Terminal.Gui.Application;

namespace Loom.UI.Terminal.Views.Dialogs;

public class AddTaskDialog : Dialog
{
    private readonly AddTask _addTask;

    private readonly TextField _titleField;
    private readonly TextView _notesView;
    private readonly TextField _dueField;

    private readonly Button _saveButton;
    private readonly Button _cancelButton;

    public bool TaskCreated { get; private set; } = false;
    public TaskItem? CreatedTask { get; private set; }

    public AddTaskDialog(AddTask addTask)
        : base()
    {
        Title = "Add New Task";
        Height = 18;
        Width = 60;
        Border.BorderStyle = LineStyle.Heavy;
        _addTask = addTask;

        // === Title ===
        var lblTitle = new Label("Title:") { X = 1, Y = 1 };
        _titleField = new TextField()
        {
            X = 12,
            Y = 1,
            Width = Dim.Fill() - 2,
        };

        // === Notes ===
        var lblNotes = new Label("Notes:") { X = 1, Y = 3 };
        _notesView = new TextView()
        {
            X = 12,
            Y = 3,
            Width = Dim.Fill() - 2,
            Height = 5,
            AllowsTab = false,
        };

        //TODO: make a note of those keybind somwhere for the user
        _notesView.KeyDown += (_, args) =>
        {
            if (args.KeyEvent.Key == (Key.CtrlMask | Key.T))
            {
                _notesView.InsertText("    ");
                args.Handled = true;
            }
        };

        // === Due Date ===
        var lblDue = new Label("Due (yyyy-mm-dd):") { X = 1, Y = 9 };
        _dueField = new TextField(DateOnly.FromDateTime(DateTime.Today).ToString("yyyy-MM-dd"))
        {
            X = 22,
            Y = 9,
            Width = 15,
        };

        // === Buttons ===
        _saveButton = new Button("Create")
        {
            IsDefault = true,
            X = Pos.Center() - 12,
            Y = Pos.Bottom(_dueField) + 2,
        };

        _cancelButton = new Button("Cancel")
        {
            X = Pos.Center() + 4,
            Y = Pos.Bottom(_dueField) + 2,
        };

        // === Save Logic ===
        _saveButton.Clicked += async (_, __) =>
        {
            var title = _titleField.Text.ToString()?.Trim() ?? "";
            var notes = _notesView.Text.ToString()?.Trim();
            DateOnly? due = null;

            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.ErrorQuery("Invalid Input", "Title is required.", "OK");
                return;
            }

            if (DateOnly.TryParse(_dueField.Text.ToString(), out var parsedDue))
                due = parsedDue;

            var task = await _addTask.Handle(title, notes, due);
            CreatedTask = task;
            TaskCreated = true;

            TuiApp.RequestStop(this);
        };

        // === Cancel Logic ===
        _cancelButton.Clicked += (_, __) => TuiApp.RequestStop(this);

        // === Add Views ===
        Add(
            lblTitle,
            _titleField,
            lblNotes,
            _notesView,
            lblDue,
            _dueField,
            _saveButton,
            _cancelButton
        );

        // === Focus ===
        _titleField.SetFocus();
    }
}
