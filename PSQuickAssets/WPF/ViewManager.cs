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

        public void ShowSettingsView()
        {
            var settingsView = App.Current.Windows.OfType<SettingsView>().FirstOrDefault();

            if (settingsView is null)
                new SettingsView().Show();
            else
                settingsView.Activate();
        }

        public void CreateAndShowMainView()
        {
            _mainViewModel = new MainViewModel(new ImageFileLoader(), this);

            MainView ??= new MainView() { DataContext = _mainViewModel };
            MainView.RestoreState();
            MainView.Show();

            MainView.FadeIn();
        }

        public void ToggleMainView()
        {
            if (MainView.IsShown)
                HideMainView();
            else
                ShowMainView();
        }

        public void ShowMainView()
        {
            MainView.FadeIn();
            MainView.Activate();
        }

        public void HideMainView()
        {
            MainView.FadeOut();
        }

        public void CloseMainView()
        {
            MainView?.SaveState();
            MainView?.Close();
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
