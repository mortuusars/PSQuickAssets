using System;
using System.Windows.Input;
using PSQuickAssets.WPF;

namespace PSQuickAssets.Services
{
    public class GlobalHotkeyRegistry
    {
        public event EventHandler<Hotkey> HotkeyRegistered;

        public Hotkey HotkeyInfo { get; private set; } = new Hotkey(Key.None, ModifierKeys.None);

        private SharpHotkeys.WPF.Hotkey _registeredSharpHotkey;

        public bool Register(Hotkey hotkey, IntPtr windowHandle, Action action, out string errorMessage)
        {
            _registeredSharpHotkey?.Dispose();
            _registeredSharpHotkey = new SharpHotkeys.WPF.Hotkey(hotkey.Key, hotkey.Modifiers, windowHandle);

            if (!_registeredSharpHotkey.TryRegisterHotkey(out uint errCode))
            {
                errorMessage = MessageFromCode(errCode, hotkey);
                return false;
            }

            HotkeyInfo = hotkey;
            _registeredSharpHotkey.HotkeyClicked += action;
            errorMessage = "";

            HotkeyRegistered?.Invoke(this, hotkey);
            return true;
        }

        public void Dispose()
        {
            _registeredSharpHotkey?.Dispose();
            HotkeyInfo = new Hotkey(Key.None, ModifierKeys.None);
        }

        private string MessageFromCode(uint errCode, Hotkey hotkey)
        {
            switch (errCode)
            {
                case 1409:
                    return $"Error registering hotkey: {hotkey} already registered.";
                default:
                    return $"Error registering hotkey <{hotkey}>. Error code: {errCode}.";
            }
        }
    }
}
