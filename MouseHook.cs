using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MousePointerIndicator;

public class MouseHook
{
    public event Action<int, int>? MiddleClick;

    private IntPtr hookID = IntPtr.Zero;

    private NativeMethods.LowLevelMouseProc proc;

    public MouseHook()
    {
        proc = HookCallback;
    }

    public void Start()
    {
        using Process curProcess = Process.GetCurrentProcess();

        using ProcessModule curModule = curProcess.MainModule!;

        hookID = NativeMethods.SetWindowsHookEx(
            NativeMethods.WH_MOUSE_LL,
            proc,
            NativeMethods.GetModuleHandle(curModule.ModuleName),
            0);
    }
    public void Stop()
    {
        if (hookID != IntPtr.Zero)
        {
            NativeMethods.UnhookWindowsHookEx(hookID);
            hookID = IntPtr.Zero;
        }
    }

    private IntPtr HookCallback(
        int nCode,
        IntPtr wParam,
        IntPtr lParam)
    {
        if (nCode >= 0)
        {
            if (wParam == (IntPtr)NativeMethods.WM_MBUTTONDOWN)
            {
                NativeMethods.GetCursorPos(out var pt);

                MiddleClick?.Invoke(pt.X, pt.Y);

                return (IntPtr)1;
            }
        }

        return NativeMethods.CallNextHookEx(
            hookID,
            nCode,
            wParam,
            lParam);
    }
}