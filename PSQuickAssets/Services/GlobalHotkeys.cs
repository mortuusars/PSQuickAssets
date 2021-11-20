using MGlobalHotkeys;
using MLogger;
using System;
using System.Collections.Generic;

namespace PSQuickAssets.Services;

internal enum HotkeyUse
{
    ToggleMainWindow
} 

/// <summary>
/// Contains all logic related to global hotkeys.
/// </summary>
internal class GlobalHotkeys : IDisposable
{
    /// <summary>
    /// Describes which action to use when registering a hotkey;
    /// </summary>
    public Dictionary<HotkeyUse, Action> HotkeyActions { get; }

    private readonly Dictionary<HotkeyUse, Hotkey> _registeredHotkeys;

    private readonly MGlobalHotkeys.GlobalHotkeys _globalHotkeysHandler;
    private readonly IntPtr _windowHandle;
    private readonly INotificationService _notificationService;
    private readonly ILogger _logger;

    internal GlobalHotkeys(IntPtr windowHandle, INotificationService notificationService, ILogger logger)
    {
        HotkeyActions = new Dictionary<HotkeyUse, Action>();
        _registeredHotkeys = new Dictionary<HotkeyUse, Hotkey>();

        _globalHotkeysHandler = new MGlobalHotkeys.GlobalHotkeys();
        _windowHandle = windowHandle;
        _notificationService = notificationService;
        _logger = logger;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="hotkey"></param>
    /// <param name="use"></param>
    internal void Register(Hotkey hotkey, HotkeyUse use)
    {
        _logger.Debug($"Registering <{hotkey}> for <{use}>");

        if (!HotkeyActions.ContainsKey(use))
        {
            _logger.Error($"Cannot register hotkey for <{use}>. No action was registered for this use.");
            return;
        }

        if (_registeredHotkeys.ContainsKey(use))
        {
            var hotkeyInUse = _registeredHotkeys[use];
            _logger.Debug($"Removing previous hotkey: <{hotkeyInUse}> for <{use}>");
            if (_globalHotkeysHandler.TryUnregister(hotkeyInUse, out string unregErrorMessage))
                _registeredHotkeys.Remove(use);
            else
            {
                _logger.Error(unregErrorMessage);
                return;
            }
        }

        if (_globalHotkeysHandler.TryRegister(hotkey, _windowHandle, HotkeyActions[use], out string regErrorMessage))
        {
            _registeredHotkeys.Add(use, hotkey);
            _logger.Info($"<{hotkey}> for <{use}> is registered.");
        }
        else
        {
            _notificationService.Notify(App.AppName, regErrorMessage, NotificationIcon.Error);
            _logger.Error(regErrorMessage);
        }
    }

    /// <summary>
    /// Is specified hotkey registered.
    /// </summary>
    internal bool IsRegistered(Hotkey hotkey)
    {
        return _registeredHotkeys.ContainsValue(hotkey) && _globalHotkeysHandler.IsRegistered(hotkey);
    }

    /// <summary>
    /// Unregisters all registered hotkeys.
    /// </summary>
    public void Dispose()
    {
        foreach (var hotkey in _registeredHotkeys)
        {
            _globalHotkeysHandler.TryUnregister(hotkey.Value, out string _);
        }
    }
}
