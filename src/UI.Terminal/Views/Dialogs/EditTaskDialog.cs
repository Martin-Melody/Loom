using Loom.Application.UseCases.Tasks;
using Loom.Core.Entities;
using Terminal.Gui;
using TuiApp = Terminal.Gui.Application;

namespace Loom.UI.Terminal.Views.Dialogs;

public class EditTaskDialog : BaseDialog
{
    private readonly EditTask _editTask;
    private readonly TaskItem _task;

    private readonly TextField _titleField;
    private readonly TextView _notesView;
    private readonly TextField _dueField;

    private readonly Button _saveButton;
    private readonly Button _cancelButton;

    public bool TaskUpdated { get; private set; } = false;

    public EditTaskDialog(EditTask editTask, TaskItem task)
        : base("Edit Task", defaultHeight: 18, maxWidth: 70)
    {
        _editTask = editTask;
        _task = task;

        var lblTitle = new Label("Title:") { X = 1, Y = 1 };
        _titleField = new TextField(task.Title)
        {
            X = 12,
            Y = 1,
            Width = Dim.Fill() - 2,
        };

        var lblNotes = new Label("Notes:") { X = 1, Y = 3 };
        _notesView = new TextView
        {
            X = 12,
            Y = 3,
            Width = Dim.Fill() - 2,
            Height = 5,
            Text = task.Notes ?? "",
            AllowsTab = false,
        };

        _notesView.KeyDown += (_, args) =>
        {
            if (args.KeyEvent.Key == (Key.CtrlMask | Key.T))
            {
                _notesView.InsertText("    ");
                args.Handled = true;
            }
        };

        _notesView.Text = task.Notes ?? "";

        var lblDue = new Label("Due:") { X = 1, Y = 9 };
        _dueField = new TextField(task.DueDate?.ToString("yyyy-MM-dd") ?? "")
        {
            X = 12,
            Y = 9,
            Width = Dim.Fill() - 2,
        };

        _saveButton = new Button("Save")
        {
            IsDefault = true,
            X = Pos.Center() - 10,
            Y = Pos.Bottom(_dueField) + 2,
        };

        _cancelButton = new Button("Cancel")
        {
            X = Pos.Center() + 2,
            Y = Pos.Bottom(_dueField) + 2,
        };

        _saveButton.Clicked += async (_, __) =>
        {
            var title = _titleField.Text.ToString() ?? "";
            var notes = _notesView.Text.ToString();
            DateOnly? due = null;
            if (DateOnly.TryParse(_dueField.Text.ToString(), out var parsed))
                due = parsed;

            await _editTask.Handle(_task.Id, title, notes, due);
            TaskUpdated = true;
            TuiApp.RequestStop(this);
        };

        _cancelButton.Clicked += (_, __) => TuiApp.RequestStop(this);

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
    }
}
