using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MousePointerIndicator;

public class KeyboardHook
{
    public event Action? EscPressed;
    public event Action? CtrlZPressed;

    private IntPtr hookID = IntPtr.Zero;

    private bool ctrlPressed = false;

    private NativeMethods.LowLevelKeyboardProc proc;

    public KeyboardHook()
    {
        proc = HookCallback;
    }

    public void Start()
    {
        using Process process = Process.GetCurrentProcess();

        using ProcessModule module = process.MainModule!;

        hookID = NativeMethods.SetWindowsHookEx(
            NativeMethods.WH_KEYBOARD_LL,
            proc,
            NativeMethods.GetModuleHandle(module.ModuleName),
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
            var info = Marshal.PtrToStructure<NativeMethods.KBDLLHOOKSTRUCT>(lParam);

            // Ctrl ����
            if (wParam == (IntPtr)NativeMethods.WM_KEYDOWN)
            {
                if (info.vkCode == NativeMethods.VK_CONTROL ||
                    info.vkCode == NativeMethods.VK_LCONTROL ||
                    info.vkCode == NativeMethods.VK_RCONTROL)
                {
                    ctrlPressed = true;
                }

                // ESC
                if (info.vkCode == NativeMethods.VK_ESCAPE)
                {
                    EscPressed?.Invoke();
                }

                // Ctrl + Z
                if (ctrlPressed &&
                    info.vkCode == NativeMethods.VK_Z)
                {
                    CtrlZPressed?.Invoke();
                }
            }

            // Ctrl ��
            if (wParam == (IntPtr)NativeMethods.WM_KEYUP)
            {
                if (info.vkCode == NativeMethods.VK_CONTROL ||
                    info.vkCode == NativeMethods.VK_LCONTROL ||
                    info.vkCode == NativeMethods.VK_RCONTROL)
                {
                    ctrlPressed = false;
                }
            }
        }

        return NativeMethods.CallNextHookEx(
            hookID,
            nCode,
            wParam,
            lParam);
    }
}