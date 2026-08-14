using AutoDarkModeApp.Contracts.Services;
using Microsoft.UI.Windowing;

namespace AutoDarkModeApp.Services;

public class CloseService(ILocalSettingsService localSettingsService) : ICloseService
{
    public void Close()
    {
        if (App.MainWindow.AppWindow.Presenter is not OverlappedPresenter presenter)
        {
            return;
        }

        var values = new List<KeyValuePair<string, object>>
        {
            new("WindowState", (int)presenter.State),
        };

        if (presenter.State == OverlappedPresenterState.Restored)
        {
            var position = App.MainWindow.AppWindow.Position;
            var size = App.MainWindow.AppWindow.Size;

            values.Add(new("X", position.X));
            values.Add(new("Y", position.Y));
            values.Add(new("Width", size.Width));
            values.Add(new("Height", size.Height));
        }

        // One write for all five values instead of five full-file rewrites in sequence.
        localSettingsService.SaveSettings(values);
    }
}
