using MGlobalHotkeys.WPF;
using PSQuickAssets.Configuration;
using PSQuickAssets.Resources;
using PSQuickAssets.Services;
using System.Windows.Input;

namespace PSQuickAssets.ViewModels
{
    public class SettingsViewModel
    {
        public Hotkey ToggleMainWindowHotkey
        {
            get => _toggleMainWindowHotkey;
            set
            {
                _toggleMainWindowHotkey = value;

                if (_config.TrySetValue(nameof(_config.ShowHideWindowHotkey), ToggleMainWindowHotkey.ToString(), out string error))
                {
                    _globalHotkeys.Register(ToggleMainWindowHotkey, HotkeyUse.ToggleMainWindow);

                    if (!_config.SavesOnPropertyChanged)
                        _config.Save();
                }
                else
                    _notificationService.Notify(App.AppName + Localization.Instance["Settings"], Localization.Instance["Settings_ToggleWindowHotkeyFailed"] + "\n" + error, NotificationIcon.Error);
            }
        }
        public bool CheckUpdates
        {
            get => _checkUpdates;
            set
            {
                _checkUpdates = value;

                if (_config.TrySetValue(nameof(_config.CheckUpdates), CheckUpdates, out string error))
                {
                    if (!_config.SavesOnPropertyChanged)
                        _config.Save();
                }
                else
                    _notificationService.Notify(App.AppName + Localization.Instance["Settings"], Localization.Instance["Settings_SavingConfigFailed"] + "\n" + error, NotificationIcon.Error);
            }
        }

        public bool AddMaskIfHasSelection
        {
            get => _addMaskIfHasSelection;
            set
            {
                _addMaskIfHasSelection = value;

                if (_config.TrySetValue(nameof(_config.AddMaskIfDocumentHasSelection), AddMaskIfHasSelection, out string error))
                {
                    if (!_config.SavesOnPropertyChanged)
                        _config.Save();
                }
                else
                    _notificationService.Notify(App.AppName + Localization.Instance["Settings"], Localization.Instance["Settings_SavingConfigFailed"] + "\n" + error, NotificationIcon.Error);
            }
        }

        public bool AlwaysOnTop
        {
            get => _alwaysOnTop;
            set
            {
                _alwaysOnTop = value;

                if (_config.TrySetValue(nameof(_config.AlwaysOnTop), AlwaysOnTop, out string error))
                {
                    if (!_config.SavesOnPropertyChanged)
                        _config.Save();
                }
                else
                    _notificationService.Notify(App.AppName + Localization.Instance["Settings"], Localization.Instance["Settings_SavingConfigFailed"] + "\n" + error, NotificationIcon.Error);
            }
        }

        //TODO: Clean up settings. Generalize config updates.

        public ICommand SaveCommand { get; }

        private Hotkey _toggleMainWindowHotkey;
        private bool _checkUpdates;
        private bool _addMaskIfHasSelection;
        private bool _alwaysOnTop;

        private readonly Config _config;
        private readonly Services.GlobalHotkeys _globalHotkeys;
        private readonly INotificationService _notificationService;

        internal SettingsViewModel(Config config, Services.GlobalHotkeys globalHotkeys, INotificationService notificationService)
        {
            _config = config;
            _globalHotkeys = globalHotkeys;
            _notificationService = notificationService;

            SaveCommand = new RelayCommand(_ => ApplyNewSettings());

            _toggleMainWindowHotkey = Hotkey.FromString(_config.ShowHideWindowHotkey);
            _checkUpdates = _config.CheckUpdates;
            _addMaskIfHasSelection = _config.AddMaskIfDocumentHasSelection;
            _alwaysOnTop = _config.AlwaysOnTop;
        }

        private void ApplyNewSettings()
        {
            if (!_config.SavesOnPropertyChanged)
                _config.Save();
        }
    }
}
