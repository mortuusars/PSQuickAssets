using System.IO;
using System.Windows;

namespace PSQuickAssets.Utils;

internal class NewGroupData
{
    public string? Name { get; set; }
    public IEnumerable<string> FilePaths { get; set; } = new List<string>();

    public NewGroupData(string? name, IEnumerable<string> filePaths)
    {
        Name = name;
        FilePaths = filePaths;
    }
}

internal static class SelectImageFIles
{
    public static Func<IEnumerable<NewGroupData>> SelectImages { get; } =
        () =>
        {
            string[] files = SystemDialogs.SelectFiles(Localize[nameof(Lang.SelectImages)], $"{FileFilters.Images} | {FileFilters.AllFiles}", SelectionMode.Multiple);

            if (files.Length == 0)
                return Array.Empty<NewGroupData>();

            IEnumerable<NewGroupData> result = new[] { new NewGroupData(null, files) };
            return CheckAndConfirmCount(result) ? result : Array.Empty<NewGroupData>();
        };

    public static Func<IEnumerable<NewGroupData>> SelectFolders { get; } =
        () =>
        {
            string[] folders = SystemDialogs.SelectFolder(Localize[nameof(Lang.SelectFolders)], SelectionMode.Multiple);

            if (folders.Length == 0)
                return Array.Empty<NewGroupData>();

            List<NewGroupData> newDatas = new();

            foreach (string path in folders)
            {
                DirectoryInfo dir = new(path);
                FileInfo[] files = dir.GetFiles();
                if (files.Length > 0)
                    newDatas.Add(new NewGroupData(dir.Name, files.Select(f => f.FullName)));
            }

            return CheckAndConfirmCount(newDatas) ? newDatas : Array.Empty<NewGroupData>();
        };

    public static Func<IEnumerable<NewGroupData>> SelectFoldersWithSubfolders { get; } =
        () =>
        {
            string[] folders = SystemDialogs.SelectFolder(Localize[nameof(Lang.SelectFolders)], SelectionMode.Multiple);

            if (folders.Length == 0)
                return Array.Empty<NewGroupData>();

            List<NewGroupData> newDatas = new();

            foreach (string path in folders)
                CreateGroupDataRecursive(newDatas, path);

            return CheckAndConfirmCount(newDatas) ? newDatas : Array.Empty<NewGroupData>();
        };

    // Recursively adds files from folders to the list.
    private static void CreateGroupDataRecursive(IList<NewGroupData> newGroupDatas, string folderPath)
    {
        DirectoryInfo dir = new(folderPath);
        FileInfo[] files = dir.GetFiles();
        if (files.Length > 0)
            newGroupDatas.Add(new NewGroupData(dir.Name, files.Select(f => f.FullName)));

        foreach (var directory in dir.GetDirectories())
        {
            CreateGroupDataRecursive(newGroupDatas, directory.FullName);
        }
    }

    /// <summary>
    /// Counts how many assets will be added (across groups) and asks user to confirm addition if count is larger than threshold.
    /// </summary>
    /// <param name="groupDatas">Source collection.</param>
    /// <param name="threshold">Count after which user will be asked to confirm</param>
    /// <returns><see langword="true"/> if asset count is less than threshold.<br></br>
    /// <see langword="true"/> if user confirms (when larger than threshold).<br></br>
    /// <see langword="false"/> otherwise.</returns>
    private static bool CheckAndConfirmCount(IEnumerable<NewGroupData> groupDatas, int threshold = 100)
    {
        int assetCount = 0;
        foreach (var data in groupDatas)
            assetCount += data.FilePaths.Count();

        if (assetCount > threshold)
        {
            string msbMessage = string.Format(Localize[nameof(Lang.Assets_Warning_LargeAmountOfAssets)], assetCount);
            if (MessageBox.Show(msbMessage, App.AppName, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                return false;
        }

        return true;
    }
}