using AsyncAwaitBestPractices;
using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Extensions.DependencyInjection;
using PSQuickAssets.Services;
using PSQuickAssets.Utils;
using System.Diagnostics;
using System.Windows;

namespace PSQuickAssets;

public partial class App : Application
{
    public static string AppName { get => "PSQuickAssets"; }
    public static Version Version { get => AppVersion.AssemblyVersion; }
    public static DateTime Build { get => AppVersion.BuildTime; }

    public static IServiceProvider ServiceProvider { get; private set; } = DIKernel.ServiceProvider;

    public App()
    {
        DispatcherUnhandledException += CrashHandler.OnUnhandledException;
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        ShutdownIfAnotherInstanceRunning();

        //Initialize task bar icon:
        var _ = (TaskbarIcon)FindResource("TaskBarIcon");

        var windowManager = ServiceProvider.GetRequiredService<WindowManager>();
        windowManager.ShowMainWindow();

        ServiceProvider.GetRequiredService<TerminalHandler>().Setup();

        var config = ServiceProvider.GetRequiredService<IConfig>();
        SetupGlobalHotkeys(windowManager, config);
        if (config.CheckUpdates)
            ServiceProvider.GetRequiredService<UpdateChecker>().CheckUpdatesAsync(Version).SafeFireAndForget();
    }

    private void SetupGlobalHotkeys(WindowManager windowManager, IConfig config)
    {
        //TODO: Move to GlobalHotkeys.
        var globalHotkeys = ServiceProvider.GetRequiredService<GlobalHotkeys>();
        globalHotkeys.HotkeyActions.Add(HotkeyUse.ToggleMainWindow, () => windowManager.ToggleMainWindow());
        globalHotkeys.Register(MGlobalHotkeys.WPF.Hotkey.FromString(config.ShowHideWindowHotkey), HotkeyUse.ToggleMainWindow);
    }

    private void ShutdownIfAnotherInstanceRunning()
    {
        Process current = Process.GetCurrentProcess();
        bool isAnotherOpen = Process.GetProcessesByName(current.ProcessName).Any(p => p.Id != current.Id);

        if (isAnotherOpen)
        {
            MessageBox.Show("Another instance of PSQuickAssets is already running.", AppName, MessageBoxButton.OK, MessageBoxImage.Information);
            App.Current.Shutdown();
            Environment.Exit(0); // Kill process if shutdown not worked. Probably only in debug.
        }
    }
}