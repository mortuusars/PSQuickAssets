using AsyncAwaitBestPractices;
using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Extensions.DependencyInjection;
using MLogger;
using MTerminal.WPF;
using PSQuickAssets.Services;
using PSQuickAssets.Update;
using PSQuickAssets.Utils;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace PSQuickAssets;

//TODO: Use this
//public class Result<TValue, TError>
//{
//    public TValue Value { get; }
//    public TError? Error { get; }

//    public bool HasValue { get => Error is null; }

//    public Result(TValue value)
//    {
//        Value = value;
//        Error = default;
//    }

//    public Result(TError error, TValue defaultValue)
//    {
//        Value = defaultValue;
//        Error = error;
//    }
//}

public partial class App : Application
{
    public static string AppName { get; } = "PSQuickAssets";
    public Version Version { get; }
    public string Build { get; }

    public static string AppDataFolder { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.AppName);

    public IServiceProvider ServiceProvider { get; private set; }

    internal IConfig Config { get; }
    public ILogger Logger { get; }

    public App()
    {
        DispatcherUnhandledException += CrashHandler.OnUnhandledException;

        Version = new Version(AppVersion.GetVersionFromAssembly());
        Build = AppVersion.GetLinkerTime(Assembly.GetEntryAssembly()!).ToString("yyMMddHHmmss");

        ServiceProvider = DIKernel.ServiceProvider;

        Config = ServiceProvider.GetRequiredService<IConfig>();
        Config.PropertyChanged += (s, e) =>
        {
            if (nameof(Config.DebugMode).Equals(e.PropertyName))
            {
                if (Logger is not null)
                    Logger.Level = Config.DebugMode ? LogLevel.Debug : LogLevel.Info;
            }
        };
        Logger = ServiceProvider.GetRequiredService<ILogger>();

    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        ShutdownIfAlreadyOpen();
        SetTooltipDelay(650);
        var _ = (TaskbarIcon)FindResource("TaskBarIcon");

        var windowManager = ServiceProvider.GetRequiredService<WindowManager>();
        windowManager.ShowMainWindow();

        Console.SetOut(Terminal.Out);
        Terminal.Commands.Add(new TerminalCommand("appdatafolder", "Opens PSQuickAssets data folder in explorer", (_) => OpenAppdataFolder()));
        Terminal.Commands.Add(new TerminalCommand("updatewindow", "Show update window", (_) => windowManager.ShowUpdateWindow(Version, new Version("99.99.99"), "Nothing changed.")));
        Terminal.Commands.Add(new TerminalCommand("exit", "Exits the app", (_) => Shutdown()));

        SetupGlobalHotkeys(windowManager);

        if (Config.CheckUpdates)
            ServiceProvider.GetRequiredService<UpdateChecker>().CheckUpdatesAsync(Version).SafeFireAndForget();
    }

    private void SetupGlobalHotkeys(WindowManager windowManager)
    {
        var globalHotkeys = ServiceProvider.GetRequiredService<GlobalHotkeys>();
        globalHotkeys.HotkeyActions.Add(HotkeyUse.ToggleMainWindow, () => windowManager.ToggleMainWindow());
        globalHotkeys.Register(MGlobalHotkeys.WPF.Hotkey.FromString(Config.ShowHideWindowHotkey), HotkeyUse.ToggleMainWindow);
    }

    private void OpenAppdataFolder()
    {
        ProcessStartInfo processStartInfo = new(App.AppDataFolder);
        processStartInfo.UseShellExecute = true;
        Process.Start(processStartInfo);
    }

    internal static void ToggleTerminalWindow()
    {
        if (!Terminal.IsOpen || !Terminal.IsVisible)
            Terminal.Show();
        else
            Terminal.Close();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        Config?.Save();
        base.OnExit(e);
    }

    private static void SetTooltipDelay(int delayMS)
    {
        ToolTipService.InitialShowDelayProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(delayMS));
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