using Loom.Core.Entities;
using Loom.Core.Entities.Enums;
using Loom.UI.Terminal.Theme;
using Terminal.Gui;

namespace Loom.UI.Terminal.Views.UI;

public sealed class NotificationView : FrameView
{
    public DateTime CreatedAt { get; }
    public TimeSpan Duration { get; }

    public NotificationView(Notification n)
        : base("")
    {
        CreatedAt = n.CreatedAt;
        Duration = n.Duration;

        CanFocus = false;

        // The FRAME itself should match the app theme, NOT the notification color
        ColorScheme = Colors.Base;
        Border.BorderStyle = LineStyle.Rounded;

        //
        // INNER COLORED PANEL (only this gets Success/Warning/Error colors)
        //
        var panel = new View()
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            ColorScheme = GetNotificationScheme(n.Level),
        };

        // MESSAGE + TIME
        var msg = new Label(n.Message) { X = 1, Y = 0 };

        var time = new Label(n.CreatedAt.ToString("HH:mm"))
        {
            X = Pos.Right(msg) + 2,
            Y = 0,
            TextAlignment = TextAlignment.Right,
        };

        panel.Add(msg, time);
        Add(panel);

        Width = n.Message.Length + time.Text.Length + 8;
        Height = 3;
    }

    private ColorScheme GetNotificationScheme(NotificationLevel level) =>
        level switch
        {
            NotificationLevel.Success => LoomColors.NotificationSuccess,
            NotificationLevel.Warning => LoomColors.NotificationWarning,
            NotificationLevel.Error => LoomColors.NotificationError,
            _ => LoomColors.NotificationInfo,
        };
}
