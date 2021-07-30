using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Hardcodet.Wpf.TaskbarNotification;
using PSQuickAssets.Infrastructure;

namespace PSQuickAssets
{
    public partial class App : Application
    {
        public static Version Version { get; private set; } = new Version("1.0.0");
        public static ViewManager ViewManager { get; private set; } = new ViewManager();
        public static TaskbarIcon TaskBarIcon { get; private set; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ShutdownIfAlreadyRunning();
            ViewManager.CreateAndShowMainView();
            CheckUpdates();
        }

        private async void CheckUpdates()
        {
            var update = await new Update.UpdateChecker().CheckAsync();
            if (update.updateAvailable)
            {
                string message = $"New version available. Visit https://github.com/mortuusars/PSQuickAssets/releases/latest to download.\n\n" +
                    $"Version: {update.versionInfo.Version}\nChangelog: {update.versionInfo.Description}";
                MessageBox.Show(message, "PSQuickAssets Update", MessageBoxButton.OK, MessageBoxImage.Information);
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
            ViewManager.SaveMainViewState();
            ((TaskbarIcon)FindResource("TaskBarIcon")).Dispose();
            ConfigManager.Write();

            base.OnExit(e);
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"PSQuickAssets has crashed.\n\n{e.Exception.Message}\n\n{e.Exception.StackTrace}");
            Shutdown();
        }
    }
}
