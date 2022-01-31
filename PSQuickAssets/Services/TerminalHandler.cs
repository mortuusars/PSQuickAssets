using MTerminal.WPF;
using PSQuickAssets.Windows;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace PSQuickAssets.Services;

internal class TerminalHandler
{
    private readonly WindowManager _windowManager;

    public TerminalHandler(WindowManager windowManager)
    {
        _windowManager = windowManager;
    }

    internal void Setup()
    {
        Terminal.Commands.Add(new TerminalCommand("errorDialog", "Test window", (_) => new ErrorDialog().Show()));
        Terminal.Commands.Add(new TerminalCommand("log", "Prints last log file contents", (_) => PrintLastLog()));
        Terminal.Commands.Add(new TerminalCommand("appdatafolder", "Opens PSQuickAssets data folder in explorer", (_) => OpenAppdataFolder()));
        Terminal.Commands.Add(new TerminalCommand("updatewindow", "Show update window", (_) => _windowManager.ShowUpdateWindow(App.Version, new Version("99.99.99"), "Nothing changed.")));
        Terminal.Commands.Add(new TerminalCommand("exit", "Exits the app", (_) => App.Current.Shutdown()));
    }

    private void PrintLastLog()
    {
        string? lastLogFilePath = Directory.GetFiles(Folders.Logs)
            .Select(f => new FileInfo(f))
            .OrderByDescending(f => f.LastWriteTime)
            .Select(f => f.FullName)
            .FirstOrDefault();

        if (string.IsNullOrWhiteSpace(lastLogFilePath))
            Terminal.WriteLine("No logs found.");
        else
        {
            string logContent = string.Empty;
            using (var stream = File.Open(lastLogFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                logContent = Encoding.UTF8.GetString(bytes);
            }
            Terminal.WriteLine(logContent);
        }
    }

    private void OpenAppdataFolder()
    {
        ProcessStartInfo processStartInfo = new(App.AppDataFolder);
        processStartInfo.UseShellExecute = true;
        Process.Start(processStartInfo);
    }
}
