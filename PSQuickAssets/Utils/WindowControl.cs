using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace PSQuickAssets.Utils
{
    public static class WindowControl
    {
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        public static bool FocusWindow(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);

            if (processes.Length > 0)
            {
                try
                {
                    SetForegroundWindow(processes[0].MainWindowHandle);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
                return false;
        }

        public static bool IsProcessRunning(string processName)
        {
            return Process.GetProcessesByName(processName).Length > 0;
        }
    }
}
