using PSQuickAssets.ViewModels;
using PSQuickAssets.Views;
using System.Linq;

namespace PSQuickAssets.Services
{
    public class WindowManager
    {
        public MainWindow MainView { get; private set; }
        private MainViewModel _mainViewModel;

        public void CreateAndShowMainWindow()
        {
            _mainViewModel = new MainViewModel(new ImageFileLoader(), this);

            MainView ??= new MainWindow() { DataContext = _mainViewModel };
            MainView.RestoreState();
            MainView.Show();

            MainView.FadeIn();
        }

        public void ToggleMainWindow()
        {
            if (MainView.IsShown)
                HideMainWindow();
            else
                ShowMainWindow();
        }

        public void ShowMainWindow()
        {
            MainView.FadeIn();
            MainView.Activate();
        }

        public void HideMainWindow()
        {
            MainView.FadeOut();
        }

        public void CloseMainWindow()
        {
            MainView?.SaveState();
            MainView?.Close();
        }

        public void ShowSettingsWindow()
        {
            var settingsWindow = App.Current.Windows.OfType<SettingsWindow>().FirstOrDefault();

            if (settingsWindow is null)
            {
                settingsWindow = new SettingsWindow();
                settingsWindow.DataContext = new SettingsViewModel(Program.GlobalHotkeys, this);
                //settingsWindow.Owner = MainView;
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
