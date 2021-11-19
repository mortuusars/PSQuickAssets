using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PSQuickAssets
{
    public partial class App : Application
    {
        public static string AppName { get; } = "PSQuickAssets";
        public static Version Version { get; } = new Version("1.2.0");
        public static string AppDataFolder { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.AppName);


        private TaskbarIcon _taskBarIcon;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (IsAnotherInstanceOpen())
            {
                MessageBox.Show("Another instance of PSQuickAssets is already running.", "PSQuickAssets", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                Shutdown();
                return;
            }

            InitTaskbarIcon();
            SetTooltipDelay(650);
            Program program = Program.Init(e.Args);
        }

        private bool IsAnotherInstanceOpen()
        {
            Process current = Process.GetCurrentProcess();
            return Process.GetProcessesByName(current.ProcessName).Any(p => p.Id != current.Id);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ConfigManager.Save();

            Program.GlobalHotkeys?.Dispose(Program.GlobalHotkeys.HotkeyInfo);
            Program.WindowManager?.CloseMainWindow();
            _taskBarIcon?.Dispose();

            base.OnExit(e);
        }

        private void InitTaskbarIcon()
        {
            _taskBarIcon = (TaskbarIcon)FindResource("TaskBarIcon");
        }

        private static void SetTooltipDelay(int delayMS)
        {
            ToolTipService.InitialShowDelayProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(delayMS));
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"PSQuickAssets has crashed.\n\n{e.Exception.Message}\n\n{e.Exception.StackTrace}");
            Shutdown();
        }
    }
}
