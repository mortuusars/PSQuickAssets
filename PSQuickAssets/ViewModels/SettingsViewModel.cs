using System.Windows.Input;
using PropertyChanged;
using PSQuickAssets.WPF;

namespace PSQuickAssets.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class SettingsViewModel
    {
        public Hotkey GlobalHotkey { get; set; }
        public bool CheckUpdates { get; set; }

        public ICommand SaveCommand { get; set; }

        public SettingsViewModel()
        {
            SaveCommand = new RelayCommand(_ => Apply());

            GlobalHotkey = App.GlobalHotkey.HotkeyInfo;
            CheckUpdates = ConfigManager.Config.CheckUpdates;
        }

        public void Apply()
        {
            ((App)App.Current).RegisterGlobalHotkey(GlobalHotkey.ToString());
            ConfigManager.Config = ConfigManager.Config with { CheckUpdates = CheckUpdates};

            ConfigManager.Save();
        }
    }
}
