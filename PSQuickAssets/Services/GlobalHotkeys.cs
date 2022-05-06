using MGlobalHotkeys.WPF;
using Serilog;
using System.Windows.Input;
using System.Windows.Interop;

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
    /// Specifies what action corresponds to HotkeyUse.
    /// </summary>
    public Dictionary<HotkeyUse, Action> HotkeyActions { get; } = new();

    private readonly Dictionary<HotkeyUse, Hotkey> _registeredHotkeys = new();
    private IntPtr? _windowHandle;

    private readonly MGlobalHotkeys.WPF.GlobalHotkeys _globalHotkeysHandler;
    private readonly INotificationService _notificationService;
    private readonly ILogger _logger;

    public GlobalHotkeys(INotificationService notificationService, ILogger logger)
    {
        _globalHotkeysHandler = new MGlobalHotkeys.WPF.GlobalHotkeys();
        _notificationService = notificationService;
        _logger = logger;
    }

    internal void InitializeHandle(IntPtr windowHandle)
    {
        _windowHandle = windowHandle;
    }
    
    /// <summary>
    /// Registers specified Hotkey with specified use. Displays an error if registering failed.
    /// </summary>
    internal void Register(Hotkey hotkey, HotkeyUse use)
    {
        if (_windowHandle is null)
            throw new InvalidOperationException("Cannot register global hotkey: window handle is not set. Call 'InitializeHandle' method to set window handle.");

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

        if (hotkey.Key is Key.None)
        {
            _logger.Debug("Hotkey <None> will not be registered.");
            return;
        }

        if (_globalHotkeysHandler.TryRegister(hotkey, (IntPtr)_windowHandle, HotkeyActions[use], out string regErrorMessage))
        {
            _registeredHotkeys.Add(use, hotkey);
            _logger.Debug($"Registered <{hotkey}> for <{use}>.");
        }
        else
        {
            _notificationService.Notify(App.AppName, regErrorMessage, NotificationIcon.Error);
            _logger.Error($"Failed to register <{hotkey}>: {regErrorMessage}");
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
            if (!_globalHotkeysHandler.TryUnregister(hotkey.Value, out string errorMsg))
                _logger.Warning("Hotkey '{0}' wasn't unregistered: {1}", hotkey.Value, errorMsg);
        }
    }
}
