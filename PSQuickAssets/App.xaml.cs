using AsyncAwaitBestPractices;
using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Extensions.DependencyInjection;
using PSQuickAssets.Services;
using PSQuickAssets.Update;
using PSQuickAssets.Utils;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace PSQuickAssets;

public partial class App : Application
{
    public static string AppName { get => "PSQuickAssets"; }
    public static Version Version { get => AppVersion.AssemblyVersion; }
    public static DateTime Build { get => AppVersion.BuildTime; }

    public static string AppDataFolder { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.AppName);

    public IServiceProvider ServiceProvider { get; private set; }

    private IConfig Config { get; }

    public App()
    {
        DispatcherUnhandledException += CrashHandler.OnUnhandledException;

        ServiceProvider = DIKernel.ServiceProvider;

        Config = ServiceProvider.GetRequiredService<IConfig>();
        ServiceProvider.GetRequiredService<ConfigChangeListener>().Listen();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        ShutdownIfAlreadyOpen();

        //Initialize task bar icon:
        var _ = (TaskbarIcon)FindResource("TaskBarIcon");

        var windowManager = ServiceProvider.GetRequiredService<WindowManager>();
        windowManager.ShowMainWindow();

        ServiceProvider.GetRequiredService<TerminalHandler>().Setup();

        SetupGlobalHotkeys(windowManager);

        if (Config.CheckUpdates)
            ServiceProvider.GetRequiredService<UpdateChecker>().CheckUpdatesAsync(Version).SafeFireAndForget();
    }

    private void SetupGlobalHotkeys(WindowManager windowManager)
    {
        //TODO: Move to GlobalHotkeys.
        var globalHotkeys = ServiceProvider.GetRequiredService<GlobalHotkeys>();
        globalHotkeys.HotkeyActions.Add(HotkeyUse.ToggleMainWindow, () => windowManager.ToggleMainWindow());
        globalHotkeys.Register(MGlobalHotkeys.WPF.Hotkey.FromString(Config.ShowHideWindowHotkey), HotkeyUse.ToggleMainWindow);
    }

    private void ShutdownIfAlreadyOpen()
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