using System.Windows;
using System.Windows.Controls;
using Hardcodet.Wpf.TaskbarNotification;
using PSQuickAssets.ViewModels;

namespace PSQuickAssets
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
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

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Program program = new Program();
            program.Startup();
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"PSQuickAssets has crashed.\n\n{e.Exception.Message}\n\n{e.Exception.StackTrace}");
        }
    }
}
