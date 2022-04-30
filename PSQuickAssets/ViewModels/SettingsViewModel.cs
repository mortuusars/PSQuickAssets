using CommunityToolkit.Mvvm.ComponentModel;
using MGlobalHotkeys.WPF;
using Microsoft.Toolkit.Mvvm.Input;
using PSQuickAssets.Services;
using PSQuickAssets.WPF;
using PureUI.Themes;
using System.ComponentModel;
using System.Windows.Input;

namespace PSQuickAssets.ViewModels;

[INotifyPropertyChanged]
internal partial class SettingsViewModel
{
    public IConfig Config { get; }
    public Hotkey ShowHideWindowHotkey
    {
        get => Hotkey.FromString(Config.ShowHideWindowHotkey);
        set
        {
            Config.ShowHideWindowHotkey = value.ToString();
            _globalHotkeys.Register(value, HotkeyUse.ToggleMainWindow);
        }
    }

    public ICollectionView Themes { get; }
    public Theme CurrentTheme
    {
        get => _themeManager.CurrentTheme;
        set
        {
            _themeManager.CurrentTheme = value;
            Config.Theme = value.Name;
        }
    }

    public IEnumerable<ThumbnailQuality> ThumbnailQualityValues { get; } = Enum.GetValues(typeof(ThumbnailQuality)).Cast<ThumbnailQuality>();

    private readonly ThemeManager _themeManager;
    private readonly Services.GlobalHotkeys _globalHotkeys;

    public SettingsViewModel(IConfig config, ThemeManager themeManager, Services.GlobalHotkeys globalHotkeys)
    {
        Config = config;
        _themeManager = themeManager;
        _globalHotkeys = globalHotkeys;

        //_themeManager.PropertyChanged += ThemeManager_PropertyChanged;
        Themes = CollectionViewSource.GetDefaultView(_themeManager.Themes);
    }

    private void ThemeManager_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(nameof(CurrentTheme));
    }
}