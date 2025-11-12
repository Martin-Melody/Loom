using Terminal.Gui;
using TuiApp = Terminal.Gui.Application;

namespace Loom.UI.Terminal.Views.Dialogs;

public class BaseDialog : Dialog
{
    public BaseDialog(string title, int defaultHeight = 20, int maxWidth = 80)
        : base()
    {
        Title = title;
        Border.BorderStyle = LineStyle.Heavy;

        UpdateLayout(defaultHeight, maxWidth);

        TuiApp.TerminalResized += (_) => UpdateLayout(defaultHeight, maxWidth);
    }

    private void UpdateLayout(int defaultHeight, int maxWidth)
    {
        int screenW = TuiApp.Driver.Cols;
        int screenH = TuiApp.Driver.Rows;

        int dialogW = Math.Min(screenW - 6, maxWidth);
        int dialogH = Math.Min(screenH - 4, defaultHeight);

        Width = dialogW;
        Height = dialogH;

        X = Pos.Center();
        Y = Pos.Center();

        SetNeedsDisplay();
    }
}
