using Microsoft.Toolkit.Mvvm.Input;
using PSQuickAssets.Resources;
using PSQuickAssets.Services;
using System.Windows.Input;

namespace PSQuickAssets.ViewModels;

internal class TaskBarViewModel
{
    private readonly WindowManager _windowManager;

    public string AppName { get; set; }
    public string ToggleWindow { get; set; }
    public string ToggleTerminalWindow { get; set; }
    public string Settings { get; set; }
    public string Exit { get; set; }

    public IConfig Config { get; }

    public ICommand ShowWindowCommand { get; }

    public ICommand ToggleTerminalCommand { get; }
    public ICommand SettingsCommand { get; }
    public ICommand ExitCommand { get; }

    public TaskBarViewModel(WindowManager windowManager, IConfig config)
    {
        AppName = App.AppName;
        ToggleWindow = Localization.Instance["Taskbar_ShowMainWindow"];
        ToggleTerminalWindow = Localization.Instance["Taskbar_ShowTerminal"];
        Settings = Localization.Instance["Taskbar_Settings"];
        Exit = Localization.Instance["Taskbar_Exit"];

        _windowManager = windowManager;
        Config = config;

        ShowWindowCommand = new RelayCommand(_windowManager.ToggleMainWindow);
        ToggleTerminalCommand = new RelayCommand(_windowManager.ToggleTerminalWindow);
        SettingsCommand = new RelayCommand(_windowManager.ToggleSettingsWindow);
        ExitCommand = new RelayCommand(App.Current.Shutdown);
    }
}