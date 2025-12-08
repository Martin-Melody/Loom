using Loom.Application.Interfaces;
using Loom.UI.Terminal.Views.Windows;

namespace Loom.UI.Terminal.Controllers;

public class NotificationHistoryController
{
    private readonly INotificationService _service;

    public NotificationHistoryController(INotificationService service)
    {
        _service = service;
    }

    public NotificationHistoryWindow CreateWindow()
    {
        return new NotificationHistoryWindow(_service);
    }

    public void DismissAll() => _service.DismissAll();

    public void DismissLast() => _service.DismissLast();
}
