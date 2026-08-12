using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

/// <summary>
/// Detects whether the user is physically logged in and the desktop is unlocked on a Windows system.
/// </summary>
public static class WindowsLoginDetector
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    [DllImport("user32.dll")]
    private static extern IntPtr GetShellWindow();

    /// <summary>
    /// Returns true only if the user is physically at the desktop and the lock screen is NOT displayed.
    /// </summary>
    public static bool IsUserFullyLoggedInAndUnlocked()
    {
        // 1. Verify whether the Explorer shell is fully loaded (desktop is present)
        IntPtr shellWindow = GetShellWindow();
        if (shellWindow == IntPtr.Zero)
        {
            return false;
        }

        // 2. Check the foreground window
        IntPtr foregroundHwnd = GetForegroundWindow();

        // On Windows 10/11 GetForegroundWindow returns IntPtr.Zero for regular processes
        // if the screen is locked or the lock overlay is active.
        if (foregroundHwnd == IntPtr.Zero)
        {
            return false;
        }

        // 3. Determine the process that owns the foreground window
        GetWindowThreadProcessId(foregroundHwnd, out uint processId);
        if (processId == 0)
        {
            return false;
        }

        try
        {
            using (Process proc = Process.GetProcessById((int)processId))
            {
                string name = proc.ProcessName;

                // If the foreground is the Windows lock application (LockApp) or the login UI (LogonUI)
                if (name.Equals("LockApp", StringComparison.OrdinalIgnoreCase) ||
                    name.Equals("LogonUI", StringComparison.OrdinalIgnoreCase) ||
                    name.Equals("Windows.UI.Xaml.Host", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }
        }
        catch
        {
            // If we cannot access the foreground window's process, we are likely still in the protected lock screen mode
            return false;
        }

        return true;
    }

    /// <summary>
    /// Blocks the thread and waits until the user physically unlocks the desktop.
    /// </summary>
    public static void WaitForUserUnlock(int checkIntervalMs = 500)
    {
        while (!IsUserFullyLoggedInAndUnlocked())
        {
            Thread.Sleep(checkIntervalMs);
        }
    }
}