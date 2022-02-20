using PSQuickAssets.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace PSQuickAssets.Commands;

internal class WindowManagerCommands
{
    public ICommand ToggleMainWindowCommand { get; }
    public ICommand OpenOrCloseSettingsCommand { get; }

    private WindowManager _windowManager;

    public WindowManagerCommands()
    {
        _windowManager = DIKernel.ServiceProvider.GetRequiredService<WindowManager>();

        ToggleMainWindowCommand = new RelayCommand(_windowManager.ToggleMainWindow);
        OpenOrCloseSettingsCommand = new RelayCommand(_windowManager.ToggleSettingsWindow);
    }
}
