using MGlobalHotkeys;
using PSQuickAssets.Configuration;
using PSQuickAssets.Services;
using System.Windows.Input;

namespace PSQuickAssets.ViewModels
{
    public class SettingsViewModel
    {
        public Hotkey ToggleMainWindowHotkey { get; set; }
        public bool CheckUpdates { get; set; }

        public ICommand SaveCommand { get; }

        private readonly Config _config;
        private readonly Services.GlobalHotkeys _globalHotkeys;
        private readonly INotificationService _notificationService;

        internal SettingsViewModel(Config config, Services.GlobalHotkeys globalHotkeys, INotificationService notificationService)
        {
            _config = config;
            _globalHotkeys = globalHotkeys;
            _notificationService = notificationService;

            SaveCommand = new RelayCommand(_ => ApplyNewSettings());

            ToggleMainWindowHotkey = Hotkey.FromString(_config.ShowHideWindowHotkey);
            CheckUpdates = _config.CheckUpdates;
        }

        private void ApplyNewSettings()
        {
            _globalHotkeys.Register(ToggleMainWindowHotkey, HotkeyUse.ToggleMainWindow);

            string failedSettings = "";

            failedSettings += _config.TrySetValue(nameof(_config.ShowHideWindowHotkey), ToggleMainWindowHotkey.ToString()).Length > 0 ? "Window Open/Close hotkey\n" : "";
            failedSettings += _config.TrySetValue(nameof(_config.CheckUpdates), CheckUpdates).Length > 0 ? "Check for Updates\n" : "";

            if (!_config.SavesOnPropertyChanged)
                _config.Save();

            if (failedSettings.Length > 0)
                _notificationService.Notify("PSQuickAssets Settings", "Failed to save one or more settings:\n" + failedSettings, NotificationIcon.Error);
        }
    }
}
