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

            ShowWindowCommand = new RelayCommand(_ => App.ViewManager.ToggleMainWindow());
            SettingsCommand = new RelayCommand(_ => App.ViewManager.ShowSettingsWindow());
            ExitCommand = new RelayCommand(_ => App.Current.Shutdown());
        }
    }
}
