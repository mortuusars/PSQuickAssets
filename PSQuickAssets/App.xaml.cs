using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using Hardcodet.Wpf.TaskbarNotification;
using PSQuickAssets.Services;
using PSQuickAssets.WPF;

namespace PSQuickAssets
{
    public partial class App : Application
    {
        public static Version Version { get; private set; } = new Version("1.1.1");

        public static GlobalHotkey GlobalHotkey { get; set; } = new GlobalHotkey();
        public static ViewManager ViewManager { get; private set; } = new ViewManager();
        public static TaskbarIcon TaskBarIcon { get; private set; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ShutdownIfAlreadyRunning();
            //ViewManager.ShowSplashView();
            ViewManager.CreateAndShowMainView();
            RegisterGlobalHotkey(ConfigManager.Config.Hotkey);

            CheckUpdates();
        }

        public void RegisterGlobalHotkey(string hotkey)
        {
            IntPtr handle = new WindowInteropHelper(ViewManager.MainView).Handle;
            if (GlobalHotkey.RegisterHotkey(new WPF.Hotkey(hotkey), handle, OnGlobalHotkey, out string errMessage))
                GlobalHotkey.WriteToConfig();
            else
                MessageBox.Show(errMessage);
        }

        private void OnGlobalHotkey() => ViewManager.ToggleMainView();

        private async void CheckUpdates()
        {
            if (!ConfigManager.Config.CheckUpdates)
                return;

            var update = await new Update.UpdateChecker().CheckAsync();
            if (update.updateAvailable)
            {
                string message = "New version available.\nVisit https://github.com/mortuusars/PSQuickAssets/releases/latest to download.\n\n" +
                    $"Version: {update.versionInfo.Version}\nChangelog:\n{update.versionInfo.Description}";
                MessageBox.Show(ViewManager.MainView, message, "PSQuickAssets Update", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

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
            ViewManager.CloseMainView();
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
