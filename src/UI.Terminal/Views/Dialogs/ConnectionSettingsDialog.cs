using Terminal.Gui;

namespace Loom.UI.Terminal.Views.Dialogs;

public sealed class ConnectionSettingsDialog : BaseDialog
{
    public RadioGroup ModeRadio { get; }
    public TextField UrlField { get; }
    public TextField ApiKeyField { get; }
    public Label StatusLabel { get; }

    public Button TestButton { get; }
    public Button SaveButton { get; }
    public Button CloseButton { get; }

    public ConnectionSettingsDialog()
        : base("Connection Settings", defaultHeight: 18, maxWidth: 70)
    {
        ModeRadio = new RadioGroup(new[] { "_Local", "_Remote" }) { X = 2, Y = 1 };

        Add(ModeRadio);

        Add(new Label("Server URL:") { X = 2, Y = 4 });

        UrlField = new TextField("")
        {
            X = 2,
            Y = 5,
            Width = 40,
        };
        Add(UrlField);

        Add(new Label("API Key:") { X = 2, Y = 7 });

        ApiKeyField = new TextField("")
        {
            X = 2,
            Y = 8,
            Width = 40,
            Secret = true,
        };
        Add(ApiKeyField);

        TestButton = new Button("Test") { X = 2, Y = 11 };
        SaveButton = new Button("Save") { X = Pos.Right(TestButton) + 2, Y = 11 };
        CloseButton = new Button("Close") { X = Pos.Right(SaveButton) + 2, Y = 11 };

        Add(TestButton, SaveButton, CloseButton);

        StatusLabel = new Label("")
        {
            X = 2,
            Y = 13,
            Width = Dim.Fill(),
        };

        Add(StatusLabel);
    }
}
