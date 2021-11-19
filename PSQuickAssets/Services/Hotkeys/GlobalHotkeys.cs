using System;
using System.Collections.Generic;
using System.Windows.Input;
using PSQuickAssets.WPF;

namespace PSQuickAssets.Services.Hotkeys
{
    public class GlobalHotkeys
    {
        public Dictionary<Hotkey, SharpHotkeys.WPF.Hotkey> Hotkeys { get; private set; }
        public Hotkey HotkeyInfo { get; private set; } = new Hotkey(Key.None, ModifierKeys.None);

        private SharpHotkeys.WPF.Hotkey _registeredSharpHotkey;

        public GlobalHotkeys()
        {
            Hotkeys = new Dictionary<Hotkey, SharpHotkeys.WPF.Hotkey>();
        }

        //public bool Register(Hotkey hotkey, Action onHotkeyPressed, out string errorMessage)
        //{

        //}

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

            return true;
        }

        public void Dispose(Hotkey hotkey)
        {
            _registeredSharpHotkey?.Dispose();
            HotkeyInfo = new Hotkey(Key.None, ModifierKeys.None);
        }

        private string MessageFromCode(uint errCode, Hotkey hotkey)
        {
            switch (errCode)
            {
                case 1409:
                    return $"Error registering hotkey: <{hotkey}> is already registered.";
                default:
                    return $"Error registering hotkey <{hotkey}>. Error code: {errCode}.";
            }
        }
    }
}
