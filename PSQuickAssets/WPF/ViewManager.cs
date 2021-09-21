using System.Linq;
using Ookii.Dialogs.Wpf;
using PSQuickAssets.ViewModels;
using PSQuickAssets.Views;

namespace PSQuickAssets.WPF
{
    public class ViewManager
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
            var settingsView = App.Current.Windows.OfType<SettingsWindow>().FirstOrDefault();

            if (settingsView is null)
                new SettingsWindow().Show();
            else
                settingsView.Activate();
        }

        public string ShowSelectDirectoryDialog()
        {
            var dialog = new VistaFolderBrowserDialog();

            if (dialog.ShowDialog() is true)
                return dialog.SelectedPath;
            else
                return string.Empty;
        }
    }
}
