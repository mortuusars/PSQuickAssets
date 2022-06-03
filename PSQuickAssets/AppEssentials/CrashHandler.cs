using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace PSQuickAssets.Services
{
    internal static class CrashHandler
    {
        internal static void OnUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;

            string message = "Unexpected crash has occured. Program will exit.";

            // Try to write crash report to a file, if failed - show message box:
            if (TrySaveCrashReport(e.Exception, out string reportFilePath))
            {
                message += $"\n\nCrash Report has been saved to: {reportFilePath}";

                if (MessageBox.Show(message + "\n\nOpen the file now?", App.AppName, MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
                {
                    ProcessStartInfo process = new(reportFilePath) { UseShellExecute = true };
                    Process.Start(process);
                }
            }
            else
            {
                message += $"\n\n{e.Exception.Message}\n\n{e.Exception.StackTrace?.Substring(0, 500)}";
                MessageBox.Show(message, App.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
            }

            App.ServiceProvider.GetRequiredService<ILogger>().Fatal(message, e.Exception);
            App.Current.Shutdown();
            Environment.Exit(-1);
        }

        private static bool TrySaveCrashReport(Exception exception, out string reportFilePath)
        {
            try
            {
                DirectoryInfo crashReportsDirectory = GetReportsFolder();
                CleanReportsFolder(crashReportsDirectory);
                reportFilePath = Path.Combine(crashReportsDirectory.FullName, $"crash-{DateTime.Now:yyyyMMdd-HHmmss}.txt");
                CreateReportString(exception)
                    .WriteToFile(reportFilePath);
                return true;
            }
            catch (Exception)
            {
                reportFilePath = string.Empty;
                return false;
            }
        }

        private static string CreateReportString(Exception ex)
        {
            return $"Crash Report - {DateTime.Now}" +
                   $"\n\n{App.AppName} Version: {App.Version}-{App.Build:yyyyMMddss}" +
                   $"\n\nMessage:\n\t{ex.Message}\n\nStackTrace:\n{ex.StackTrace}\n\n{ex.InnerException}";
        }

        private static DirectoryInfo GetReportsFolder()
        {
            var directory = new DirectoryInfo(Folders.CrashReports);
            if (!directory.Exists)
                directory.Create();
            return directory;
        }

        private static void CleanReportsFolder(DirectoryInfo crashReportsDirectory)
        {
            try
            {
                // Leave 10 most recent files - delete rest:
                crashReportsDirectory.GetFiles()
                    .OrderByDescending(f => f.CreationTime)
                    .Skip(10)
                    .ForEach(file => file.Delete());
            }
            catch (DirectoryNotFoundException) { }
            catch (IOException) { }
        }
    }
}
