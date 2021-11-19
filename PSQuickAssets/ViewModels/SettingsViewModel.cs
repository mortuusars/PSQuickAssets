using System.Timers;
using System.Windows;
using System.Windows.Input;
using PropertyChanged;
using PSQuickAssets.Services;
using PSQuickAssets.Services.Hotkeys;
using PSQuickAssets.WPF;

namespace PSQuickAssets.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class SettingsViewModel
    {

        public Hotkey GlobalHotkey { get; set; }
        public bool CheckUpdates { get; set; }

        public string SavedMessage { get; set; }

        public ICommand SaveCommand { get; }

        private readonly GlobalHotkeys _globalHotkeys;
        private readonly WindowManager _windowManager;

        public SettingsViewModel(GlobalHotkeys globalHotkeys, WindowManager windowManager)
        {
            _globalHotkeys = globalHotkeys;
            _windowManager = windowManager;
            SaveCommand = new RelayCommand(_ => ApplyNewSettings());

            GlobalHotkey = new Hotkey(ConfigManager.Config.Hotkey);
            CheckUpdates = ConfigManager.Config.CheckUpdates;
        }

        private void ApplyNewSettings()
        {
            _globalHotkeys.Remove(new Hotkey(ConfigManager.Config.Hotkey));
            if (!_globalHotkeys.TryRegister(GlobalHotkey, () => _windowManager.ToggleMainWindow(), out string errorMessage))
            {
                GlobalHotkey = new Hotkey(ConfigManager.Config.Hotkey);
                MessageBox.Show(errorMessage);
                return;
            }

            ConfigManager.Config = ConfigManager.Config with
            {
                Hotkey = GlobalHotkey.ToString(),
                CheckUpdates = CheckUpdates
            };

            ConfigManager.Save();

            SavedMessage = "Saved";
            Timer timer = new Timer(1000);
            timer.Elapsed += (s, e) => { SavedMessage = ""; timer.Stop(); };
            timer.Start();
        }
    }
}
