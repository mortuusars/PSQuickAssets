using MGlobalHotkeys.WPF;
using Microsoft.Toolkit.Mvvm.Input;
using PSQuickAssets.Services;
using System.Windows.Input;

namespace PSQuickAssets.ViewModels;

internal class SettingsViewModel
{
    public Hotkey ToggleMainWindowHotkey
    {
        get => Hotkey.FromString(Config.ShowHideWindowHotkey);
        set
        {
            Config.ShowHideWindowHotkey = value.ToString();
            _globalHotkeys.Register(value, HotkeyUse.ToggleMainWindow);
        }
    }

    public IConfig Config { get; }

    public ICommand SaveCommand { get; }

    private readonly Services.GlobalHotkeys _globalHotkeys;

    public SettingsViewModel(IConfig config, Services.GlobalHotkeys globalHotkeys)
    {
        Config = config;
        _globalHotkeys = globalHotkeys;

        SaveCommand = new RelayCommand(Config.Save);
    }
}