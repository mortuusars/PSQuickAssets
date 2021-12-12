using MLogger;
using MLogger.LogWriters;
using MLogger.Terminal;
using MLogger.Terminal.Models;
using MLogger.Terminal.Views;
using PSQuickAssets.ViewModels;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;

namespace PSQuickAssets.Services;

internal class MLoggerSetup
{
    private static Terminal? _terminal;

    private readonly INotificationService _notificationService;

    internal MLoggerSetup(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    internal ILogger CreateLogger()
    {
        ILogger logger = new Logger(LogLevel.Debug);

        try
        {
            string logsFolder = Path.Combine(App.AppDataFolder, "logs");
            Directory.CreateDirectory(logsFolder);
            logger.Loggers.Add(new FileWriter(Path.Combine(logsFolder, "log.txt")));
            logger.Loggers.Add(new DebugWriter());

            _terminal = new Terminal("PSQuickAssets Terminal");
            _terminal.Commands
                .Add(new ConsoleCommand("appdatafolder", "Opens PSQuickAssets data folder in explorer", (_) => OpenAppdataFolder()))
                .Add(new ConsoleCommand("saveassets", "Save", (_) => ((MainViewModel)App.WindowManager!.MainWindow!.DataContext).AssetsViewModel.SaveJson()))
                .Add(new ConsoleCommand("exit", "Exits the app", (_) => App.Current.Shutdown()));

            logger.Loggers.Add(_terminal);
        }
        catch (Exception ex)
        {
            _notificationService.Notify(App.AppName, $"Error has occured during Logging Setup. Click to show details.", NotificationIcon.Warning,
                () => MessageBox.Show($"Error in Logging Setup:\n\n{ex}", App.AppName, MessageBoxButton.OK, MessageBoxImage.Warning));
        }

        return logger;
    }

    internal static void ToggleTerminalWindow()
    {
        if (_terminal is null)
            return;

        if (App.Current.Windows.OfType<TerminalWindow>().FirstOrDefault() is not null)
            _terminal.CloseWindow();
        else
            _terminal.ShowWindow();
    }

    private void OpenAppdataFolder()
    {
        ProcessStartInfo processStartInfo = new(App.AppDataFolder);
        processStartInfo.UseShellExecute = true;
        Process.Start(processStartInfo);
    }
}
