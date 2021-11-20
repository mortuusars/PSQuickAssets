using MGlobalHotkeys;
using PropertyChanged;
using PSQuickAssets.Services;
using System.Windows.Input;

namespace PSQuickAssets.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class SettingsViewModel
    {
        public Hotkey ToggleMainWindowHotkey { get; set; }
        public bool CheckUpdates { get; set; }

        public ICommand SaveCommand { get; }

        private readonly Services.GlobalHotkeys _globalHotkeys;

        internal SettingsViewModel(Services.GlobalHotkeys globalHotkeys)
        {
            _globalHotkeys = globalHotkeys;
            SaveCommand = new RelayCommand(_ => ApplyNewSettings());

            ToggleMainWindowHotkey = Hotkey.FromString(ConfigManager.Config.Hotkey);
            CheckUpdates = ConfigManager.Config.CheckUpdates;
        }

        private void ApplyNewSettings()
        {
            _globalHotkeys.Register(ToggleMainWindowHotkey, HotkeyUse.ToggleMainWindow);

            ConfigManager.Config = ConfigManager.Config with
            {
                Hotkey = ToggleMainWindowHotkey.ToString(),
                CheckUpdates = CheckUpdates
            };

            ConfigManager.Save();
        }
    }
}
