using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using Hardcodet.Wpf.TaskbarNotification;
using PSQuickAssets.Services;
using PSQuickAssets.ViewModels;
using PSQuickAssets.Views;
using PSQuickAssets.WPF;

namespace PSQuickAssets
{
    public partial class App : Application
    {
        public static Version Version { get; } = new Version("1.2.0");

        public static GlobalHotkey GlobalHotkey { get; set; } = new GlobalHotkey();
        public static ViewManager ViewManager { get; private set; } = new ViewManager();
        public static TaskbarIcon TaskBarIcon { get; private set; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ShutdownIfAlreadyRunning();

            ViewManager.CreateAndShowMainWindow();
            RegisterGlobalHotkey(ConfigManager.Config.Hotkey);

            new Update.Update().CheckUpdatesAsync();
        }

        public void RegisterGlobalHotkey(string hotkey)
        {
            IntPtr handle = new WindowInteropHelper(ViewManager.MainView).Handle;
            if (GlobalHotkey.RegisterHotkey(new WPF.Hotkey(hotkey), handle, OnGlobalHotkey, out string errMessage))
                GlobalHotkey.WriteToConfig();
            else
                MessageBox.Show(errMessage);
        }

        private void OnGlobalHotkey() => ViewManager.ToggleMainWindow();

        private void ShutdownIfAlreadyRunning()
        {
            var current = Process.GetCurrentProcess();

            foreach (var process in Process.GetProcessesByName(current.ProcessName))
            {
                if (process.Id != current.Id && process.MainModule.FileName == current.MainModule.FileName)
                {
                    MessageBox.Show("Another instance of PSQuickAssets is already running", "PSQuickAssets", MessageBoxButton.OK, MessageBoxImage.Information);
                    Shutdown();
                }
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            TaskBarIcon = (TaskbarIcon)FindResource("TaskBarIcon");
            ToolTipService.InitialShowDelayProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(650));
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ConfigManager.Save();

            GlobalHotkey.Dispose();
            ViewManager.CloseMainWindow();
            TaskBarIcon.Dispose();

            base.OnExit(e);
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"PSQuickAssets has crashed.\n\n{e.Exception.Message}\n\n{e.Exception.StackTrace}");
            Shutdown();
        }
    }
}
