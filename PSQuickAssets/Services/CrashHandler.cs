using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;

namespace PSQuickAssets.Services
{
    internal static class CrashHandler
    {
        internal static void OnUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;

            string message = $"Unexpected crash has occured. Program will exit.";

            if (TrySaveCrashReport(e.Exception, out string reportFilePath) && File.Exists(reportFilePath))
            {
                message += $"\n\nCrash Report has been saved to: {reportFilePath}";

                if (MessageBox.Show(message + "\n\nOpen the file now?", App.AppName, MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
                {
                    ProcessStartInfo process = new ProcessStartInfo(reportFilePath) { UseShellExecute = true };
                    Process.Start(process);
                }
            }
            else
            {
                message += $"\n\n{e.Exception.Message}\n\n{e.Exception.StackTrace?.Substring(0, 500)}";
                MessageBox.Show(message, App.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
            }

            try
            {
                App.Logger?.Fatal(message, e.Exception);
            }
            catch (Exception) { }

            App.Current.Shutdown();
            Environment.Exit(-1);
        }

        private static bool TrySaveCrashReport(Exception exception, out string reportFilePath)
        {
            try
            {
                string reportsFolder = GetReportsFolder();
                CleanReportsFolder(reportsFolder);

                reportFilePath = Path.Combine(reportsFolder, $"crash-{DateTime.Now:yyyyMMdd-HHmmss}.txt");

                string report = $"Crash Report - {DateTime.Now}" +
                            $"\n\n{App.AppName} Version: {App.Version}-{App.Build}" +
                            $"\n\nMessage:\n\t{exception.Message}\n\nStackTrace:\n{exception.StackTrace}\n\n{exception.InnerException}";

                File.WriteAllText(reportFilePath, report);

                return true;
            }
            catch (Exception)
            {
                reportFilePath = string.Empty;
                return false;
            }
        }

        private static string GetReportsFolder()
        {
            string reportsFolder = Path.Combine(App.AppDataFolder, "crash-reports");
            Directory.CreateDirectory(reportsFolder);
            return reportsFolder;
        }

        private static void CleanReportsFolder(string reportsFolder)
        {
            try
            {
                string[] reports = Directory.GetFiles(reportsFolder);

                if (reports.Length > 20)
                {
                    reports.OrderByDescending(f => File.GetCreationTime(f)).Take(5).ToList().ForEach(file => File.Delete(file));
                }
            }
            catch (Exception) { }
        }
    }
}
