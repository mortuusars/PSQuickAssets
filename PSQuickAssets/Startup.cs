using AsyncAwaitBestPractices;
using PSQuickAssets.Services;

namespace PSQuickAssets;

internal class Startup
{
    private readonly ThemeService _themeService;
    private readonly WindowManager _windowManager;
    private readonly TerminalHandler _terminalHandler;
    private readonly GlobalHotkeys _globalHotkeys;
    private readonly IConfig _config;
    private readonly UpdateChecker _updateChecker;

    public Startup(ThemeService themeService, WindowManager windowManager, TerminalHandler terminalHandler, 
        GlobalHotkeys globalHotkeys, IConfig config, UpdateChecker updateChecker)
    {
        _themeService = themeService;
        _windowManager = windowManager;
        _terminalHandler = terminalHandler;
        _globalHotkeys = globalHotkeys;
        _config = config;
        _updateChecker = updateChecker;
    }

    /// <summary>
    /// Configures and initializes stuff required for app to run.
    /// </summary>
    internal void Start()
    {
        _themeService.SetupThemes();
        _terminalHandler.Initialize();
        _windowManager.ShowMainWindow();

        var showWindowHotkey = MGlobalHotkeys.WPF.Hotkey.FromString(_config.ShowHideWindowHotkey);
        _globalHotkeys.InitializeHandle(_windowManager.GetMainWindowHandle());
        _globalHotkeys.HotkeyActions.Add(HotkeyUse.ToggleMainWindow, _windowManager.ShowHideMainWindow);
        _globalHotkeys.Register(showWindowHotkey, HotkeyUse.ToggleMainWindow);

        if (_config.CheckUpdates)
            _updateChecker.CheckUpdatesAsync(App.Version).SafeFireAndForget();
    }
}
