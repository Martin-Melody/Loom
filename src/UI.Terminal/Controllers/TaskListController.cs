using Loom.Application.DTOs.Tasks;
using Loom.Application.Interfaces;
using Loom.Core.Entities;

namespace Loom.UI.Terminal.Controllers;

public class TaskListController
{
    private readonly ITaskService _service;
    private readonly INotificationService _notify;
    private List<TaskView> _views = new();

    private TaskFilter? _currentFilter;
    public TaskFilter? CurrentFilter => _currentFilter;

    public event Action<IReadOnlyList<TaskView>>? TasksUpdated;
    public event Action<string>? ViewLabelChanged;
    public event Func<TaskItem?, AddTaskRequest?>? RequestTaskAdd;
    public event Func<TaskItem, EditTaskRequest?>? RequestTaskEdit;
    public event Func<TaskFilter?, TaskFilter?>? RequestTaskFilter;
    public event Func<string, string, bool>? RequestConfirmDelete;

    private int _selectedIndex = -1;
    public int SelectedIndex => _selectedIndex;

    public bool HasSelection => _selectedIndex >= 0 && _selectedIndex < _views.Count;
    public TaskView? SelectedView => HasSelection ? _views[_selectedIndex] : null;
    public TaskItem? SelectedTask => SelectedView?.Item;

    public void SetSelectedIndex(int index)
    {
        if (index >= 0 && index < _views.Count)
            _selectedIndex = index;
    }

    public TaskListController(ITaskService service, INotificationService notify)
    {
        _service = service;
        _notify = notify;
    }

    public async Task LoadTasksAsync(TaskFilter? filter = null)
    {
        // Remember which task was selected before reload
        var previousId = SelectedTask?.Id;

        _currentFilter = filter;
        var items = await _service.GetTasksAsync(filter);
        _views = items.ToList();

        // Try to restore previous selection by ID
        if (previousId.HasValue)
        {
            var newIndex = _views.FindIndex(v => v.Item.Id == previousId.Value);
            if (newIndex >= 0)
                _selectedIndex = newIndex;
            else
                _selectedIndex = Math.Min(_selectedIndex, _views.Count - 1);
        }
        else
        {
            _selectedIndex = -1;
        }

        TasksUpdated?.Invoke(_views);
        ViewLabelChanged?.Invoke(CurrentViewLabel);
    }

    public string CurrentViewLabel => _currentFilter == null ? "Today’s Tasks" : "Filtered Tasks";

    public async Task AddTaskAsync()
    {
        var request = RequestTaskAdd?.Invoke(null);
        if (request is null)
            return;

        await _service.AddTaskAsync(request);
        await LoadTasksAsync(_currentFilter);
        _notify.Success("Task Added!");
    }

    public async Task EditTaskAsync(TaskItem item)
    {
        var request = RequestTaskEdit?.Invoke(item);
        if (request is null)
            return;

        await _service.UpdateTaskAsync(request);
        await LoadTasksAsync(_currentFilter);
        _notify.Success("Task Edited!");
    }

    public async Task DeleteTaskAsync(TaskItem item)
    {
        if (
            RequestConfirmDelete == null
            || !RequestConfirmDelete("Confirm Delete", $"Delete '{item.Title}'?")
        )
            return;

        await _service.DeleteTaskAsync(item.Id);
        await LoadTasksAsync(_currentFilter);
        _notify.Warn("Task Deleted!");
    }

    public async Task ToggleCompleteAsync(TaskItem item)
    {
        await _service.ToggleCompleteAsync(item.Id);
        await LoadTasksAsync(_currentFilter);
        _notify.Info("Task Toggled!");
    }

    public void ToggleExpandCollapse()
    {
        if (!HasSelection || SelectedView is null)
            return;

        // Flip expanded state for the selected task view
        SelectedView.IsExpanded = !SelectedView.IsExpanded;

        // Notify UI to re-render the list
        TasksUpdated?.Invoke(_views);
    }

    public async Task FilterTasksAsync()
    {
        var filter = RequestTaskFilter?.Invoke(_currentFilter);
        if (filter != null)
            await LoadTasksAsync(filter);
    }
}
