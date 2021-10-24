using System.Linq;
using Ookii.Dialogs.Wpf;
using PSQuickAssets.ViewModels;
using PSQuickAssets.Views;

namespace PSQuickAssets.Services
{
    public class ViewManager
    {
        public MainWindow MainView { get; private set; }
        private MainViewModel _mainViewModel;

        private readonly IOpenDialogService _openDialogService;

        public ViewManager(IOpenDialogService openDialogService)
        {
            _openDialogService = openDialogService;
        }

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

        public string[] ShowSelectFilesDialog(string title, string filter, bool selectMultiple)
        {
            return _openDialogService.ShowSelectFilesDialog(title, filter, selectMultiple);
        }

        public string ShowSelectFolderDialog()
        {
            return _openDialogService.ShowSelectFolderDialog();
        }
    }
}
