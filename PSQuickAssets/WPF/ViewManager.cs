using System.Linq;
using Ookii.Dialogs.Wpf;
using PSQuickAssets.ViewModels;
using PSQuickAssets.Views;

namespace PSQuickAssets.WPF
{
    public class ViewManager
    {
        public MainView MainView { get; private set; }
        private MainViewModel _mainViewModel;
        private const string _MAIN_VIEW_STATE_FILE = "state.json";

        public static void ShowSplashView()
        {
            new SplashView().Show();
        }

        public static void ShowSettingsView()
        {
            var settingsView = App.Current.Windows.OfType<SettingsView>().FirstOrDefault();

            if (settingsView is null)
                new SettingsView().Show();
            else
                settingsView.Activate();
        }

        public void CreateAndShowMainView()
        {
            _mainViewModel = new MainViewModel(new ImageFileLoader(), new PhotoshopManager());

            MainView ??= new MainView() { DataContext = _mainViewModel };
            MainView.RestoreState(_MAIN_VIEW_STATE_FILE);
            MainView.Show();
        }

        public void ToggleMainView() => _mainViewModel.IsWindowShowing = !_mainViewModel.IsWindowShowing;

        public void CloseMainView()
        {
            MainView?.SaveState(_MAIN_VIEW_STATE_FILE);
            MainView?.Close();
        }

        public static string ShowSelectDirectoryDialog()
        {
            var dialog = new VistaFolderBrowserDialog();

            if (dialog.ShowDialog() is true)
                return dialog.SelectedPath;
            else
                return string.Empty;
        }
    }
}
