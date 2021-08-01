using System.Windows.Input;
using PropertyChanged;
using PSQuickAssets.WPF;

namespace PSQuickAssets.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class SettingsViewModel
    {
        public Hotkey GlobalHotkey { get; set; } = App.GlobalHotkey.HotkeyInfo;

        public ICommand SaveCommand { get; set; }

        public SettingsViewModel()
        {
            SaveCommand = new RelayCommand(_ => Apply());
        }

        public void Apply()
        {
            ((App)App.Current).RegisterGlobalHotkey(GlobalHotkey.ToString());
        }
    }
}
