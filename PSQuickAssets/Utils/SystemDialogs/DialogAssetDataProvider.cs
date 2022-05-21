using PSQuickAssets.Models;
using PSQuickAssets.Services;
using System.IO;
using System.Windows;

namespace PSQuickAssets.Utils;

public class DialogAssetDataProvider
{
    public Func<IEnumerable<NewGroupData>> ImagesDialogDataProvider { get; } = () => SelectFilesDialog();
    public Func<IEnumerable<NewGroupData>> FoldersDialogDataProvider { get; } = () => SelectFoldersDialog();
    public Func<IEnumerable<NewGroupData>> FoldersWithSubfoldersDialogDataProvider { get; } = () => SelectFoldersDialog(includeSubfolders: true);

    public static IEnumerable<NewGroupData> SelectFilesDialog()
    {
        string[] files = SystemDialogs.SelectFiles(Localize[nameof(Lang.SelectImages)],
            $"{FileFilters.Images} | {FileFilters.AllFiles}", SelectionMode.Multiple);
        return CreateNewGroupDatas(files);
    }

    public static IEnumerable<NewGroupData> SelectFoldersDialog(bool includeSubfolders = false)
    {
        string[] folders = SystemDialogs.SelectFolder(Localize[nameof(Lang.SelectFolders)], SelectionMode.Multiple);
        return CreateNewGroupDatas(folders, includeSubfolders);
    }

    private static IEnumerable<NewGroupData> CreateNewGroupDatas(string[] paths, bool includeSubfolders = false)
    {
        if (paths.Length == 0)
            return Enumerable.Empty<NewGroupData>();

        NewGroupDataCreator groupDataCreator = new()
        {
            ThresholdValidation = ShowMessageBoxIfAboveThreshhold
        };

        return groupDataCreator.Create(paths, includeSubfolders);
    }

    private static bool ShowMessageBoxIfAboveThreshhold(int count)
    {
        if (count > 150)
        {
            string msg = string.Format(Localize[nameof(Lang.Assets_Warning_LargeAmountOfAssets)], count);
            Window owner = App.Current.MainWindow;
            return MessageBox.Show(owner, msg, App.AppName, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;
        }

        return true;
    }
}
