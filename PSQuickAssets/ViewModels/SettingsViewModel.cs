using MGlobalHotkeys.WPF;
using Microsoft.Toolkit.Mvvm.Input;
using PSQuickAssets.Services;
using PSQuickAssets.WPF;
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

    public IEnumerable<ThumbnailQuality> ThumbnailQualityValues { get; set; }

    public IConfig Config { get; }

    public ICommand SaveCommand { get; }

    private readonly Services.GlobalHotkeys _globalHotkeys;

    public SettingsViewModel(IConfig config, Services.GlobalHotkeys globalHotkeys)
    {
        Config = config;
        _globalHotkeys = globalHotkeys;

        ThumbnailQualityValues = Enum.GetValues(typeof(ThumbnailQuality)).Cast<ThumbnailQuality>();

        SaveCommand = new RelayCommand(Config.Save);
    }
}