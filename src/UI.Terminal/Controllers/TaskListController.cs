using Loom.Application.DTOs.Tasks;
using Loom.Application.UseCases.Tasks;
using Loom.Core.Entities;
using Loom.UI.Terminal.Views;
using Loom.UI.Terminal.Views.Dialogs;
using Terminal.Gui;
using TuiApp = Terminal.Gui.Application;

namespace Loom.UI.Terminal.Controllers;

public class TaskListController
{
    private readonly AddTask _addTask;
    private readonly EditTask _editTask;
    private readonly DeleteTask _deleteTask;
    private readonly ToggleCompleteTask _toggleComplete;
    private readonly FilterTasks _filterTasks;

    private readonly ListView _list;
    private List<TaskView> _views = new();

    private TaskFilter? _currentFilter;
    public TaskFilter? CurrentFilter => _currentFilter;
    public bool HasSelection => _list.SelectedItem >= 0;
    public event Action<string>? ViewLabelChanged;

    public string CurrentViewLabel =>
        _currentFilter == null ? "Today’s Tasks"
        : string.IsNullOrEmpty(_currentFilter.TextContains)
        && _currentFilter.DueBefore == null
        && _currentFilter.IsComplete == null
            ? "All Tasks"
        : "Filtered Tasks";

    public TaskListController(
        ListView list,
        AddTask addTask,
        EditTask editTask,
        DeleteTask deleteTask,
        ToggleCompleteTask toggleComplete,
        FilterTasks filterTasks
    )
    {
        _list = list;
        _addTask = addTask;
        _editTask = editTask;
        _deleteTask = deleteTask;
        _toggleComplete = toggleComplete;
        _filterTasks = filterTasks;
    }

    // === Load & Refresh ===
    public async Task LoadTasks(TaskFilter? filter = null)
    {
        _currentFilter = filter;
        var items = await _filterTasks.Handle(filter);
        UpdateViews(items);
    }

    private async Task ReloadAsync()
    {
        await LoadTasks(_currentFilter);
    }

    private void UpdateViews(IEnumerable<TaskItem> items)
    {
        _views = items.Select(it => new TaskView(it)).ToList();
        RefreshList();
        ViewLabelChanged?.Invoke(CurrentViewLabel);
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

    // === Dialogs & Commands ===
    public async Task FilterTasks()
    {
        var dlg = new FilterTasksDialog();
        TuiApp.Run(dlg);

        if (!dlg.Applied)
            return;

        _currentFilter = dlg.Filter;
        var filtered = await _filterTasks.Handle(dlg.Filter);
        UpdateViews(filtered);
    }

    // TODO: Wrap these async method in Try-Catch Block to catch errors.
    public async Task AddTask()
    {
        var dlg = new AddTaskDialog(_addTask);
        TuiApp.Run(dlg);

        if (dlg.TaskCreated)
            await ReloadAsync();
    }

    public async Task EditSelectedTask()
    {
        if (!TryGetSelected(out var selected))
            return;

        var dlg = new EditTaskDialog(_editTask, selected.Item);
        TuiApp.Run(dlg);

        if (dlg.TaskUpdated)
            await ReloadAsync();
    }

    public async Task DeleteSelectedTask()
    {
        if (!TryGetSelected(out var selected))
            return;

        var confirm = MessageBox.Query(
            "Confirm Delete",
            "Are you sure you want to delete this task?",
            "Yes",
            "No"
        );

        if (confirm != 0)
            return;

        await _deleteTask.Handle(selected.Item.Id);
        await ReloadAsync();
    }

    public async Task ToggleCompleteSelected()
    {
        if (!TryGetSelected(out var selected))
            return;

        await _toggleComplete.Handle(selected.Item.Id);
        await ReloadAsync();
    }

    // === View Interaction ===
    public void ToggleExpandCollapse()
    {
        if (!TryGetSelected(out var selected))
            return;

        selected.IsExpanded = !selected.IsExpanded;
        RefreshList(_list.SelectedItem);
    }

    private bool TryGetSelected(out TaskView selected)
    {
        selected = null!;
        if (_list.SelectedItem < 0)
            return false;

        var found = FindTaskAtIndex(_list.SelectedItem);
        if (found is null)
            return false;

        selected = found;
        return true;
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
