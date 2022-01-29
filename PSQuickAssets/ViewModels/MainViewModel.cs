using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using PSQuickAssets.Services;
using PSQuickAssets.WPF;
using System.Windows.Input;

namespace PSQuickAssets.ViewModels;

internal class MainViewModel : ObservableObject
{
    public AssetsViewModel AssetsViewModel { get; }
    public IConfig Config { get; }

    public ICommand IncreaseThumbnailSizeCommand { get; }
    public ICommand DecreaseThumbnailSizeCommand { get; }

    public ICommand SettingsCommand { get; }
    public ICommand ToggleTerminalCommand { get; }
    public ICommand HideCommand { get; }
    public ICommand ShutdownCommand { get; }

    private readonly WindowManager _windowManager;

    public MainViewModel(AssetsViewModel assetsViewModel, WindowManager windowManager, IConfig config)
    {
        AssetsViewModel = assetsViewModel;
        Config = config;
        _windowManager = windowManager;

        IncreaseThumbnailSizeCommand = new RelayCommand(() => ChangeThumbnailSize(MouseWheelDirection.Up));
        DecreaseThumbnailSizeCommand = new RelayCommand(() => ChangeThumbnailSize(MouseWheelDirection.Down));
        SettingsCommand = new RelayCommand(_windowManager.ToggleSettingsWindow);
        ToggleTerminalCommand = new RelayCommand(_windowManager.ToggleTerminalWindow);
        HideCommand = new RelayCommand(_windowManager.HideMainWindow);
        ShutdownCommand = new RelayCommand(App.Current.Shutdown);
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public MainViewModel() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private void ChangeThumbnailSize(MouseWheelDirection direction)
    {
        switch (direction)
        {
            case MouseWheelDirection.Up:
                if (Config.ThumbnailSize <= 142)
                    Config.ThumbnailSize += 8;
                break;
            case MouseWheelDirection.Down:
                if (Config.ThumbnailSize >= 30)
                    Config.ThumbnailSize -= 8;
                break;
        }
    }
}