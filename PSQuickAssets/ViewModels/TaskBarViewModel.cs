using PSQuickAssets.Resources;
using PSQuickAssets.Services;
using System.Windows.Input;

namespace PSQuickAssets.ViewModels
{
    public class TaskBarViewModel
    {
        public string AppName { get; set; }
        public string ToggleTerminalWindow { get; set; }
        public string Settings { get; set; }
        public string Exit { get; set; }

        public ICommand ShowWindowCommand { get; }
        public ICommand ToggleTerminalCommand { get; }

        public ICommand SettingsCommand { get; }
        public ICommand ExitCommand { get; }

        public TaskBarViewModel()
        {
            AppName = App.AppName;
            ToggleTerminalWindow = Localization.Instance["Taskbar_ToggleTerminal"];
            Settings = Localization.Instance["Settings"];
            Exit = Localization.Instance["Taskbar_Exit"];

            ShowWindowCommand = new RelayCommand(_ => App.WindowManager.ToggleMainWindow());
            ToggleTerminalCommand = new RelayCommand(_ => MLoggerSetup.ToggleTerminalWindow());
            SettingsCommand = new RelayCommand(_ => App.WindowManager.ShowSettingsWindow());
            ExitCommand = new RelayCommand(_ => App.Current.Shutdown());
        }
    }
}
