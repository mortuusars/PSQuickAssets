using Microsoft.Toolkit.Mvvm.Input;
using PSQuickAssets.Resources;
using PSQuickAssets.Services;
using System.Windows.Input;

namespace PSQuickAssets.ViewModels;

internal class TaskBarViewModel
{
    public IConfig Config { get; }

    public ICommand ShowWindowCommand { get; }
    public ICommand SettingsCommand { get; }

    private readonly WindowManager _windowManager;
    
    public TaskBarViewModel(WindowManager windowManager, IConfig config)
    {
        _windowManager = windowManager;
        Config = config;

        ShowWindowCommand = new RelayCommand(_windowManager.ToggleMainWindow);
        SettingsCommand = new RelayCommand(_windowManager.ToggleSettingsWindow);
    }
}