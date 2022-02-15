using PSQuickAssets.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace PSQuickAssets.Commands;

internal class WindowManagerCommands
{
    public ICommand OpenOrCloseSettingsCommand { get; }

    private WindowManager _windowManager;

    public WindowManagerCommands()
    {
        _windowManager = DIKernel.ServiceProvider.GetRequiredService<WindowManager>();

        OpenOrCloseSettingsCommand = new RelayCommand(_windowManager.ToggleSettingsWindow);
    }
}
