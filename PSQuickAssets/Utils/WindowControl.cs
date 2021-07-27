using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;

namespace PSQuickAssets.Utils
{
    public static class WindowControl
    {
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        public static void FocusPSWindow()
        {
            Process[] processes = Process.GetProcessesByName("photoshop");

            if (processes.Length > 0)
            {
                try
                {
                    SetForegroundWindow(processes[0].MainWindowHandle);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to bring PS to foreground:\n{ex.Message}");
                }
            }
                
        }

        public static bool IsProcessRunning(string processName)
        {
            return Array.Exists(Process.GetProcesses(), p => p.ProcessName == processName);
        }
    }
}
