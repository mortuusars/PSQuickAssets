using MTerminal.WPF;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Windows.Media;

namespace PSQuickAssets;

internal class MTerminalSink : ILogEventSink
{
    private readonly IFormatProvider? _formatProvider;

    public MTerminalSink(IFormatProvider? formatProvider = null)
    {
        _formatProvider = formatProvider;
    }

    public void Emit(LogEvent logEvent)
    {
        if (!Terminal.IsOpen)
            return;

        var message = logEvent.RenderMessage(_formatProvider);
        Color color = ColorFromLevel(logEvent.Level);
        Terminal.WriteLine($"[{logEvent.Timestamp:HH:mm:ss}] [{logEvent.Level}] - {message}", color);
    }

    private Color ColorFromLevel(LogEventLevel level)
    {
        return level switch
        {
            LogEventLevel.Verbose => Terminal.Style.Foreground,
            LogEventLevel.Debug => Terminal.Style.Foreground,
            LogEventLevel.Information => Colors.Lavender,
            LogEventLevel.Warning => Colors.SandyBrown,
            LogEventLevel.Error => Colors.IndianRed,
            LogEventLevel.Fatal => Colors.OrangeRed,
            _ => throw new ArgumentOutOfRangeException(nameof(level))
        };
    }
}

internal static class MTerminalSinkExtensions
{
    public static LoggerConfiguration Terminal(
              this LoggerSinkConfiguration loggerConfiguration,
              IFormatProvider? formatProvider = null)
    {
        return loggerConfiguration.Sink(new MTerminalSink(formatProvider));
    }
}
