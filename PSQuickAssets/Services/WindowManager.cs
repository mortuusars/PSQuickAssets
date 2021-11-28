using PSQuickAssets.Configuration;
using PSQuickAssets.ViewModels;
using PSQuickAssets.Windows;
using System.Linq;

namespace PSQuickAssets.Services
{
    public class WindowManager
    {
        public MainWindow? MainWindow { get; private set; }
        private MainViewModel? _mainViewModel;
        private readonly INotificationService _notificationService;
        private readonly Config _config;

        internal WindowManager(INotificationService notificationService, Config config)
        {
            _notificationService = notificationService;
            _config = config;
        }

        public void CreateAndShowMainWindow()
        {
            _mainViewModel = new MainViewModel(new ImageFileLoader(), this, _notificationService, _config);

            MainWindow ??= new MainWindow() { DataContext = _mainViewModel };
            MainWindow.RestoreState();
            MainWindow.Show();

            MainWindow.FadeIn();
        }

        public void ToggleMainWindow()
        {
            if (MainWindow?.IsShown is true)
                HideMainWindow();
            else
                ShowMainWindow();
        }

        public void ShowMainWindow()
        {
            MainWindow?.FadeIn();
            MainWindow?.Activate();
        }

        public void HideMainWindow()
        {
            MainWindow?.FadeOut();
        }

        public void CloseMainWindow()
        {
            MainWindow?.SaveState();
            MainWindow?.Close();
        }

        public void ShowSettingsWindow()
        {
            var settingsWindow = App.Current.Windows.OfType<SettingsWindow>().FirstOrDefault();

            if (settingsWindow is null)
            {
                settingsWindow = new SettingsWindow();
                settingsWindow.DataContext = new SettingsViewModel(App.Config, App.GlobalHotkeys, _notificationService);
                settingsWindow.Show();
                settingsWindow.Left = WpfScreenHelper.MouseHelper.MousePosition.X;
                settingsWindow.Top = WpfScreenHelper.MouseHelper.MousePosition.Y;
            }
            else
            {
                settingsWindow.Close();
            }
        }
    }
}
