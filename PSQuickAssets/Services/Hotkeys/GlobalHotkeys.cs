using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Interop;
using MLogger;
using PSQuickAssets.Views;
using PSQuickAssets.WPF;

namespace PSQuickAssets.Services.Hotkeys
{
    public class GlobalHotkeys
    {
        public Dictionary<string, SharpHotkeys.WPF.Hotkey> Hotkeys { get; private set; }
        public Hotkey HotkeyInfo { get; private set; } = new Hotkey(Key.None, ModifierKeys.None);

        private SharpHotkeys.WPF.Hotkey _registeredSharpHotkey;
        
        private readonly ILogger _logger;

        public GlobalHotkeys(ILogger logger)
        {
            Hotkeys = new Dictionary<string, SharpHotkeys.WPF.Hotkey>();
            _logger = logger;
        }

        public bool TryRegister(Hotkey hotkey, Action onHotkeyPressed, out string errorMessage)
        {
            if (hotkey.Key == Key.None)
            {
                errorMessage = $"<{hotkey}> is not a valid hotkey.";
                _logger.Error(errorMessage);
                return false;
            }

            _logger.Debug($"Registering <{hotkey}>...");

            if (Hotkeys.ContainsKey(hotkey.ToString()))
                Remove(hotkey);

            errorMessage = string.Empty;

            var mainWindow = App.Current.Windows.OfType<MainWindow>().First();
            var windowHandle = new WindowInteropHelper(mainWindow).Handle;
            var newHotkey = new SharpHotkeys.WPF.Hotkey(hotkey.Key, hotkey.Modifiers, windowHandle);

            if (!newHotkey.TryRegisterHotkey(out uint errCode))
            {
                errorMessage = $"Failed to register hotkey <{hotkey}>.\n{MessageFromCode(errCode, hotkey)}";
                _logger.Error(errorMessage);
                return false;
            }

            newHotkey.HotkeyClicked += onHotkeyPressed;
            Hotkeys.Add(hotkey.ToString(), newHotkey);
            _logger.Info($"<{hotkey}> registered successfully!");
            return true;
        }

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

        public void Remove(Hotkey hotkey)
        {
            try
            {
                var existing = Hotkeys[hotkey.ToString()];

                if (!existing.TryUnregisterHotkey(out uint errCode))
                    _logger.Debug($"Failed to unregister <{hotkey}>: {MessageFromCode(errCode, hotkey)}");

                existing.Dispose();
                Hotkeys.Remove(hotkey.ToString());
                _logger.Info($"Unregistered and removed hotkey: <{hotkey}>");
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to remove global hotkey:\n" + ex.Message, ex);
            }
        }

        private string MessageFromCode(uint errCode, Hotkey hotkey)
        {
            switch (errCode)
            {
                case 1409:
                    return $"<{hotkey}> is already registered.";
                default:
                    return $"Error code: {errCode}.";
            }
        }
    }
}
