using System.Windows.Input;

namespace PSQuickAssets.ViewModels
{
    public class TaskBarViewModel
    {
        public string AppName { get; set; }

        public ICommand ShowWindowCommand { get; }
        public ICommand SettingsCommand { get; }
        public ICommand ExitCommand { get; }

        public TaskBarViewModel()
        {
            AppName = "PSQuickAssets " + App.Version.ToString();

            ShowWindowCommand = new RelayCommand(_ => App.ViewManager.ToggleMainView());
            SettingsCommand = new RelayCommand(_ => App.ViewManager.ShowSettingsView());
            ExitCommand = new RelayCommand(_ => App.Current.Shutdown());
        }
    }
}
