using Loom.Application.Interfaces;
using Loom.Core.Entities;
using Loom.Core.Entities.Enums;
using Terminal.Gui;

namespace Loom.UI.Terminal.Views.Windows;

public class NotificationHistoryWindow : Window
{
    private readonly INotificationService _notifications;
    private readonly ListView _list;

    public NotificationHistoryWindow(INotificationService service)
    {
        Title = "Notification History";
        Border.BorderStyle = LineStyle.None;
        _notifications = service;

        Width = Dim.Fill();
        Height = Dim.Fill();

        _list = new ListView(FormatHistory(_notifications.History))
        {
            X = 1,
            Y = 1,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            AllowsMarking = false,
            BorderStyle = LineStyle.Rounded,
        };

        Add(_list);
    }

    private List<string> FormatHistory(IReadOnlyList<Notification> history)
    {
        return history
            .Reverse()
            .Select(n =>
            {
                var icon = n.Level switch
                {
                    NotificationLevel.Success => "✔",
                    NotificationLevel.Warning => "⚠",
                    NotificationLevel.Error => "✖",
                    _ => "ℹ",
                };

                return $"{icon} [{n.CreatedAt:HH:mm}] {n.Message}";
            })
            .ToList();
    }
}
