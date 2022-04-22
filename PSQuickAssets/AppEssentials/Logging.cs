using Serilog;
using Serilog.Core;
using System.IO;

namespace PSQuickAssets.Services;

internal static class Logging
{
    public static LoggingLevelSwitch LogLevelSwitch { get; } = new LoggingLevelSwitch(Serilog.Events.LogEventLevel.Information);

    internal static ILogger CreateLogger()
    {
        return new LoggerConfiguration()
            .MinimumLevel.ControlledBy(LogLevelSwitch)
            .WriteTo.Debug()
            .WriteTo.Terminal()
            .WriteTo.File(
                Path.Combine(Folders.Logs, "log.log"),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 10,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                shared: true) // Set shared to be able to pring log in a terminal.
            .CreateLogger();
    }
}