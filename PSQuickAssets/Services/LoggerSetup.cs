using MLogger;
using MLogger.LogWriters;
using MTerminal.WPF;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace PSQuickAssets.Services;

internal static class LoggerSetup
{
    internal static ILogger CreateLogger(LogLevel logLevel, INotificationService notificationService)
    {
        ILogger logger = new Logger(logLevel);
        
        logger.Loggers.Add(new DebugWriter());
        logger.Loggers.Add(new TerminalLogger());

        try
        {
            string logsFolder = Path.Combine(App.AppDataFolder, "logs");
            Directory.CreateDirectory(logsFolder);
            string logFilePath = Path.Combine(logsFolder, "log.txt");
            logger.Loggers.Add(new FileWriter(logFilePath));
        }
        catch (Exception ex)
        {
            notificationService.Notify(App.AppName, $"Error has occured during Logging Setup. Click to show details.", NotificationIcon.Warning,
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
            //_ => Terminal.ForegroundColor
        };

        Terminal.WriteLine(message, color);
    }
}
