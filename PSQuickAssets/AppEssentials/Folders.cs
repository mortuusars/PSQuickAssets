using System.IO;

namespace PSQuickAssets;

internal static class Folders
{
    public static string AppData { get => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.AppName); }

    public static string CrashReports { get => Path.Combine(AppData, "crash-reports"); }
    public static string AssetCatalog { get => Path.Combine(AppData, "catalog"); }
    public static string Logs { get => Path.Combine(AppData, "logs"); }
}
