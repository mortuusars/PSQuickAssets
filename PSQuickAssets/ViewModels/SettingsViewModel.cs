using MGlobalHotkeys.WPF;
using Microsoft.Toolkit.Mvvm.Input;
using PSQuickAssets.Configuration;
using PSQuickAssets.Resources;
using PSQuickAssets.Services;
using System.Windows.Input;

namespace PSQuickAssets.ViewModels
{
    internal class SettingsViewModel
    {
        public Hotkey ToggleMainWindowHotkey
        {
            get => Hotkey.FromString(_config.ShowHideWindowHotkey);
            set
            {
                if (SetConfigValue(nameof(_config.ShowHideWindowHotkey), value.ToString()))
                    _globalHotkeys.Register(ToggleMainWindowHotkey, HotkeyUse.ToggleMainWindow);
            }
        }

        public bool AlwaysOnTop
        {
            get => _config.AlwaysOnTop;
            set => SetConfigValue(nameof(_config.AlwaysOnTop), value);
        }

        public bool AddMaskIfDocumentHasSelection
        {
            get => _config.AddMaskIfDocumentHasSelection;
            set => SetConfigValue(nameof(_config.AddMaskIfDocumentHasSelection), value);
        }

        public bool CheckUpdates
        {
            get => _config.CheckUpdates;
            set => SetConfigValue(nameof(_config.CheckUpdates), value);
        }

        public ICommand SaveCommand { get; }

        private readonly Config _config;
        private readonly Services.GlobalHotkeys _globalHotkeys;
        private readonly INotificationService _notificationService;

        internal SettingsViewModel(Config config, Services.GlobalHotkeys globalHotkeys, INotificationService notificationService)
        {
            _config = config;
            _globalHotkeys = globalHotkeys;
            _notificationService = notificationService;

            SaveCommand = new RelayCommand(ApplyNewSettings);
        }

        private bool SetConfigValue(string propertyName, object newValue)
        {
            if (!_config.TrySetValue(propertyName, newValue, out string error))
            {
                _notificationService.Notify(App.AppName + Localization.Instance["Settings"],
                    Localization.Instance["Settings_SavingConfigFailed"] + "\n" + error, NotificationIcon.Error);
                return false;
            }

            ApplyNewSettings();
            return true;
        }

        private void ApplyNewSettings()
        {
            if (!_config.SaveOnPropertyChanged)
                _config.Save();
        }
    }
}
