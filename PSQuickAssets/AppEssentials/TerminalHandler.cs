using MTerminal.WPF;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace PSQuickAssets.Services;

internal class TerminalHandler
{
    private readonly WindowManager _windowManager;

    public TerminalHandler(WindowManager windowManager)
    {
        _windowManager = windowManager;
    }

    internal void Initialize()
    {
#if DEBUG
        Terminal.Commands.Add(new TerminalCommand("updatewindow", "Shows test update window", (_) => _windowManager.ShowUpdateWindow(App.Version, new Version("15.5.0"))));
#endif

        Terminal.Commands.Add(new TerminalCommand("log", "Prints last log file contents", (_) => PrintLastLog()));
        Terminal.Commands.Add(new TerminalCommand("appdatafolder", "Opens PSQuickAssets data folder in explorer", (_) => OpenAppdataFolder()));
        Terminal.Commands.Add(new TerminalCommand("exit", "Exits the app", (_) => App.Current.Shutdown()));
    }

    private void PrintLastLog()
    {
        FileInfo? lastLogFile = Directory.GetFiles(Folders.Logs)
            .Select(f => new FileInfo(f))
            .OrderByDescending(f => f.LastWriteTime)
            .FirstOrDefault();

        if (string.IsNullOrWhiteSpace(lastLogFile?.FullName))
            Terminal.WriteLine("No logs found.");
        else
        {
            string logContent = string.Empty;
            
            using (var stream = lastLogFile.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                logContent = Encoding.UTF8.GetString(bytes);
            }
            Terminal.WriteLine(logContent.Substring(0, Math.Min(logContent.Length, 12000)));
        }
    }

    private void OpenAppdataFolder()
    {
        ProcessStartInfo processStartInfo = new(Folders.AppData);
        processStartInfo.UseShellExecute = true;
        Process.Start(processStartInfo);
    }
}
