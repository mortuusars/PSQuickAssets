using System;
using System.IO;

namespace PSQuickAssets;

internal class Folders
{
    public static string AppdataFolder { get => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.AppName); }
    public static string Logs { get => Path.Combine(AppdataFolder, "logs"); }
}
