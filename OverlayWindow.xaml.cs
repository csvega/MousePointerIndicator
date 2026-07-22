using System.Windows;

namespace MousePointerIndicator;

public partial class OverlayWindow : Window
{
    public OverlayWindow()
    {
        InitializeComponent();
    }

    public void ShowAt(int x, int y, ArrowDirection direction)
    {
        if (direction == ArrowDirection.Up)
        {
            UpLayout.Visibility = Visibility.Visible;
            DownLayout.Visibility = Visibility.Collapsed;

            Left = x - Width / 2;
            Top = y - 15;
        }
        else
        {
            UpLayout.Visibility = Visibility.Collapsed;
            DownLayout.Visibility = Visibility.Visible;

            Left = x - Width / 2;
            Top = y - Height + 15;
        }

        Opacity = 1;

        Show();
    }

}