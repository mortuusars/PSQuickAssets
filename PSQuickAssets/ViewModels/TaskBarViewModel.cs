using Microsoft.Toolkit.Mvvm.Input;
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

        public TaskBarViewModel(WindowManager windowManager)
        {
            AppName = App.AppName;
            ToggleTerminalWindow = Localization.Instance["Taskbar_ToggleTerminal"];
            Settings = Localization.Instance["Settings"];
            Exit = Localization.Instance["Taskbar_Exit"];

            ShowWindowCommand = new RelayCommand(windowManager.ToggleMainWindow);
            ToggleTerminalCommand = new RelayCommand(App.ToggleTerminalWindow);
            SettingsCommand = new RelayCommand(windowManager.ToggleSettingsWindow);
            ExitCommand = new RelayCommand(App.Current.Shutdown);
        }
    }
}
