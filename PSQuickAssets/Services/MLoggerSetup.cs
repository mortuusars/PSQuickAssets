using MLogger;
using MLogger.LogWriters;
using MTerminal.WPF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;

namespace PSQuickAssets.Services;

internal class MLoggerSetup
{
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
            logger.Loggers.Add(new TerminalLogger());
        }
        catch (Exception ex)
        {
            _notificationService.Notify(App.AppName, $"Error has occured during Logging Setup. Click to show details.", NotificationIcon.Warning,
                () => MessageBox.Show($"Error in Logging Setup:\n\n{ex}", App.AppName, MessageBoxButton.OK, MessageBoxImage.Warning));
        }

        return logger;
    }
}

internal class TerminalLogger : ILogWriter
{
    public void Log(IEntry logEntry)
    {
        string message = $"[{DateTime.Now:HH:mm:ss}] - [{logEntry.Level}] - {logEntry.Message}";

        Color color = logEntry.Level switch
        {
            LogLevel.Debug => Colors.LightGray,
            LogLevel.Info => Colors.LightBlue,
            LogLevel.Warning => Colors.LightYellow,
            LogLevel.Error => Colors.Red,
            LogLevel.Fatal => Colors.OrangeRed,
            _ => Terminal.ForegroundColor
        };

        Terminal.WriteLine(message, color);
    }
}
