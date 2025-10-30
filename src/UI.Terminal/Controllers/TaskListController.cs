using Terminal.Gui;
using Loom.Application.UseCases.Tasks;
using Loom.UI.Terminal.Views;
using Loom.UI.Terminal.Views.Dialogs;
using TuiApp = Terminal.Gui.Application;
using Loom.Application.DTOs.Tasks;

namespace Loom.UI.Terminal.Controllers;

public class TaskListController
{
    private readonly CreateTask _createTask;
    private readonly EditTask _editTask;
    private readonly DeleteTask _deleteTask;
    private readonly ToggleCompleteTask _toggleComplete;
    private readonly FilterTasks _filterTasks;

    private readonly ListView _list;
    private List<TaskView> _views = new();

    private TaskFilter? _currentFilter;
    public TaskFilter? CurrentFilter => _currentFilter;

    public event Action<string>? ViewLabelChanged;
    public string CurrentViewLabel =>
    _currentFilter == null
        ? "Today’s Tasks"
        : string.IsNullOrEmpty(_currentFilter.TextContains)
          && _currentFilter.DueBefore == null
          && _currentFilter.IsComplete == null
            ? "All Tasks"
            : "Filtered Tasks";



    public TaskListController(
        ListView list,
        CreateTask createTask,
        EditTask editTask,
        DeleteTask deleteTask,
        ToggleCompleteTask toggleComplete,
        FilterTasks filterTasks)
    {
        _list = list;
        _createTask = createTask;
        _editTask = editTask;
        _deleteTask = deleteTask;
        _toggleComplete = toggleComplete;
        _filterTasks = filterTasks;
    }


    public async Task LoadTasks(TaskFilter? filter = null)
    {
        _currentFilter = filter;
        var items = await _filterTasks.Handle(filter);
        _views = items.Select(it => new TaskView(it)).ToList();
        RefreshList();
        ViewLabelChanged?.Invoke(CurrentViewLabel);
    }

    private async Task RefreshCurrentView()
    {
        await LoadTasks(_currentFilter);
    }

    public void RefreshList(int? previousIndex = null)
    {
        var oldIndex = previousIndex ?? _list.SelectedItem;
        var rendered = _views.SelectMany(v => v.RenderLines()).ToList();
        _list.SetSource(rendered);

        if (rendered.Count == 0)
        {
            _list.SelectedItem = -1;
            return;
        }

        _list.SelectedItem = Math.Min(oldIndex, rendered.Count - 1);
        _list.SetFocus();
    }

    public async Task FilterTasks()
    {
        var dlg = new FilterTasksDialog();
        TuiApp.Run(dlg);

        if (!dlg.Applied)
            return;

        _currentFilter = dlg.Filter;

        var filtered = await _filterTasks.Handle(dlg.Filter);
        _views = filtered.Select(it => new TaskView(it)).ToList();
        RefreshList();

        ViewLabelChanged?.Invoke(CurrentViewLabel);
    }

    public async Task AddTask()
    {
        var dlg = new AddTaskDialog(_createTask);
        TuiApp.Run(dlg);
        if (dlg.TaskCreated)
            await RefreshCurrentView();
    }

    public async Task EditSelectedTask()
    {
        if (_list.SelectedItem < 0) return;

        var selected = FindTaskAtIndex(_list.SelectedItem);
        if (selected is null) return;

        var dlg = new EditTaskDialog(_editTask, selected.Item);
        TuiApp.Run(dlg);
        if (dlg.TaskUpdated)
            await RefreshCurrentView();
    }

    public async Task DeleteSelectedTask()
    {
        if (_list.SelectedItem < 0) return;

        var confirm = MessageBox.Query("Confirm Delete",
            "Are you sure you want to delete this task?",
            "Yes", "No");

        if (confirm == 0)
        {
            var selected = FindTaskAtIndex(_list.SelectedItem);
            if (selected is null) return;

            await _deleteTask.Handle(selected.Item.Id);
            await RefreshCurrentView();
        }
    }

    public async Task ToggleCompleteSelected()
    {
        if (_list.SelectedItem < 0) return;

        var selected = FindTaskAtIndex(_list.SelectedItem);
        if (selected is null) return;

        await _toggleComplete.Handle(selected.Item.Id);
        await RefreshCurrentView();
    }

    public void ToggleExpandCollapse()
    {
        var selected = FindTaskAtIndex(_list.SelectedItem);
        if (selected is null) return;

        selected.IsExpanded = !selected.IsExpanded;
        RefreshList(_list.SelectedItem);
    }

    private TaskView? FindTaskAtIndex(int index)
    {
        int running = 0;
        foreach (var v in _views)
        {
            var lines = v.RenderLines().ToList();
            if (index >= running && index < running + lines.Count)
                return v;
            running += lines.Count;
        }
        return null;
    }

    public IReadOnlyList<TaskView> Views => _views;
}

