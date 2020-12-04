using System.Windows;
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
            base.OnStartup(e);

        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            ((TaskbarIcon)FindResource("TaskBarIcon")).Dispose();

            ConfigManager.Write();
        }
    }
}
