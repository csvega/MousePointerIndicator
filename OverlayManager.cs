using System.Collections.Generic;
using System.Windows;
using Application = System.Windows.Application;

namespace MousePointerIndicator;

public class OverlayManager
{
    private readonly List<OverlayWindow> overlays = new();

    public void Show(int x, int y)
    {
        double screenHeight = SystemParameters.PrimaryScreenHeight;

        ArrowDirection direction =
            y < screenHeight / 2
                ? ArrowDirection.Up
                : ArrowDirection.Down;

        var overlay = new OverlayWindow();

        overlays.Add(overlay);

        Application.Current.Dispatcher.Invoke(() =>
        {
            overlay.ShowAt(x, y, direction);
        });
    }

    public void ClearAll()
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            foreach (var item in overlays)
            {
                item.Close();
            }

            overlays.Clear();
        });
    }
    public void RemoveLast()
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            if (overlays.Count == 0)
                return;

            OverlayWindow last = overlays[^1];

            last.Close();

            overlays.RemoveAt(overlays.Count - 1);
        });
    }
}