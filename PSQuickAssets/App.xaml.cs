using System;
using System.Windows;
using System.Windows.Controls;
using Hardcodet.Wpf.TaskbarNotification;
using PSQuickAssets.Infrastructure;
using PSQuickAssets.ViewModels;

namespace PSQuickAssets
{
    public partial class App : Application
    {
        public static readonly Version Version = new Version("1.0.0");
        public static readonly ViewManager ViewManager = new ViewManager();

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ViewManager.CreateAndShowMainView();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            ((TaskbarIcon)FindResource("TaskBarIcon")).DataContext = new TaskBarViewModel();
            ToolTipService.InitialShowDelayProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(650));
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            ((TaskbarIcon)FindResource("TaskBarIcon")).Dispose();

            ConfigManager.Write();
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"PSQuickAssets has crashed.\n\n{e.Exception.Message}\n\n{e.Exception.StackTrace}");
            Shutdown();
        }
    }
}
