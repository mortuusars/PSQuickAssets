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

        ServiceProvider.GetRequiredService<Startup>()
            .Start();
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