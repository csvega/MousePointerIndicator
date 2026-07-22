using System.Drawing;
using System.Windows;
using System.Windows.Forms;

namespace MousePointerIndicator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private readonly MouseHook hook = new();
        private readonly OverlayManager overlay = new();
        private readonly KeyboardHook keyboardHook = new();

        private NotifyIcon? trayIcon;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            hook.MiddleClick += (x, y) => overlay.Show(x, y);
            keyboardHook.EscPressed += () => overlay.ClearAll();
            keyboardHook.CtrlZPressed += () => overlay.RemoveLast();

            hook.Start();
            keyboardHook.Start();

            InitTrayIcon();
        }

        private void InitTrayIcon()
        {
            var menu = new ContextMenuStrip();
            menu.Items.Add("종료", null, (_, _) => Shutdown());

            trayIcon = new NotifyIcon
            {
                Icon = LoadTrayIcon(),
                Text = "Mouse Pointer Indicator",
                Visible = true,
                ContextMenuStrip = menu
            };
        }

        private static Icon LoadTrayIcon()
        {
            string? exePath = Environment.ProcessPath;

            if (exePath != null)
            {
                Icon? icon = Icon.ExtractAssociatedIcon(exePath);

                if (icon != null)
                    return icon;
            }

            return SystemIcons.Application;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            hook.Stop();
            keyboardHook.Stop();

            if (trayIcon != null)
            {
                trayIcon.Visible = false;
                trayIcon.Dispose();
                trayIcon = null;
            }

            base.OnExit(e);
        }
    }
}
