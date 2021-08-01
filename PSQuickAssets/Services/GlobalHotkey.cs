using System;
using System.Windows.Input;
using PSQuickAssets.WPF;

namespace PSQuickAssets.Services
{
    public class GlobalHotkey
    {
        public SharpHotkeys.WPF.Hotkey Hotkey { get; set; }
        public Hotkey HotkeyInfo { get; set; } = new WPF.Hotkey(Key.None, ModifierKeys.None);

        public bool RegisterHotkey(Hotkey hotkey, IntPtr windowHandle, Action action, out string errorMessage)
        {
            Hotkey?.Dispose();
            Hotkey = new SharpHotkeys.WPF.Hotkey(hotkey.Key, hotkey.Modifiers, windowHandle);

            if (!Hotkey.TryRegisterHotkey(out uint errCode))
            {
                errorMessage = MessageFromCode(errCode);
                return false;
            }

            HotkeyInfo = hotkey;

            Hotkey.HotkeyClicked += action;

            errorMessage = "";
            return true;
        }

        public void Dispose()
        {
            Hotkey?.Dispose();
            HotkeyInfo = new Hotkey(Key.None, ModifierKeys.None);
        }

        public void WriteToConfig()
        {
            ConfigManager.Config = ConfigManager.Config with { Hotkey = HotkeyInfo.ToString()};
            ConfigManager.Save();
        }

        private string MessageFromCode(uint errCode)
        {
            return errCode.ToString();
        }
    }
}
