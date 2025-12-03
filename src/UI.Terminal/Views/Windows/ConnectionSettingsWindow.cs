using Terminal.Gui;

namespace Loom.UI.Terminal.Views.Windows;

public sealed class ConnectionSettingsWindow : Window
{
    public RadioGroup ModeRadio { get; }
    public TextField UrlField { get; }
    public TextField ApiKeyField { get; }
    public Button TestButton { get; }
    public Button SaveButton { get; }
    public Button CloseButton { get; }
    public Label StatusLabel { get; }

    public ConnectionSettingsWindow()
        : base()
    {
        Title = "Connection Settings";
        Width = Dim.Percent(60);
        Height = Dim.Percent(40);
        X = Pos.Center();
        Y = Pos.Center();

        ModeRadio = new RadioGroup(2, 1, new[] { "_Local", "_Remote" }, 0);

        UrlField = new TextField("")
        {
            X = 2,
            Y = 4,
            Width = 40,
        };

        ApiKeyField = new TextField("")
        {
            X = 2,
            Y = 6,
            Width = 40,
            Secret = true,
        };

        TestButton = new Button("Test") { X = 2, Y = 8 };

        SaveButton = new Button("Save") { X = Pos.Right(TestButton) + 2, Y = 8 };

        CloseButton = new Button("Close") { X = Pos.Right(SaveButton) + 2, Y = 8 };

        StatusLabel = new Label("")
        {
            X = 2,
            Y = 10,
            Width = Dim.Fill(),
        };

        Add(
            new Label("Mode:") { X = 2, Y = 1 },
            ModeRadio,
            new Label("Server URL:") { X = 2, Y = 3 },
            UrlField,
            new Label("API Key:") { X = 2, Y = 5 },
            ApiKeyField,
            TestButton,
            SaveButton,
            CloseButton,
            StatusLabel
        );
    }
}
