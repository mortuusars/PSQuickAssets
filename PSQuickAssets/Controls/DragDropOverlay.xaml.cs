using PSQuickAssets.Models;
using PSQuickAssets.Services;
using System.IO;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PSQuickAssets.Controls;

public enum DropDataInfo
{
    None,
    Files,
    Folders,
    FilesAndFolders
}

public enum DropAction
{
    None,
    Files,
    Folders,
    FoldersAndSubfolders
}

public partial class DragDropOverlay : UserControl
{
    /// <summary>
    /// Indicates what data would be dropped.
    /// </summary>
    public DropDataInfo DropDataInfo
    {
        get { return (DropDataInfo)GetValue(DropDataInfoProperty); }
        set { SetValue(DropDataInfoProperty, value); }
    }

    public static readonly DependencyProperty DropDataInfoProperty =
        DependencyProperty.Register(nameof(DropDataInfo), typeof(DropDataInfo), typeof(DragDropOverlay), new PropertyMetadata(DropDataInfo.None));

    public ICommand AddAssetsCommand
    {
        get { return (ICommand)GetValue(AddAssetsCommandProperty); }
        set { SetValue(AddAssetsCommandProperty, value); }
    }

    public static readonly DependencyProperty AddAssetsCommandProperty =
        DependencyProperty.Register(nameof(AddAssetsCommand), typeof(ICommand), typeof(DragDropOverlay), new PropertyMetadata(null));

    public DragDropOverlay()
    {
        InitializeComponent();
    }

    private void DropArea_Drop(object sender, DragEventArgs e)
    {
        if (!e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            SystemSounds.Asterisk.Play();
            return;
        }

        string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);

        DropAction dropAction = sender.CastTo<FrameworkElement>()
            .Tag.CastTo<DropAction>();

        NewGroupDataCreator groupDataCreator = new();
        groupDataCreator.ThresholdValidation = ShowMessageBoxIfAboveThreshhold;

        IEnumerable<NewGroupData> newGroupDatas = dropAction switch
        {
            DropAction.None => throw new InvalidOperationException("DropAction.None is not a valid action for adding assets."),
            DropAction.Files or DropAction.Folders => groupDataCreator.Create(paths),
            DropAction.FoldersAndSubfolders => groupDataCreator.Create(paths, includeSubfolders: true),
            _ => throw new NotSupportedException(),
        };

        AddAssetsCommand.Execute(() => newGroupDatas);
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

    private void Root_PreviewDragEnter(object sender, DragEventArgs e)
    {
        DropDataInfo = DropDataInfo.None;

        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);

            var (directories, files) = GetFoldersAndFilesFromPaths(paths);

            if (files.Count > 0 && directories.Count > 0)
                DropDataInfo = DropDataInfo.FilesAndFolders;
            else if (files.Count > 0)
                DropDataInfo = DropDataInfo.Files;
            else if (directories.Count > 0)
                DropDataInfo = DropDataInfo.Folders;
        }
        else
            e.Effects = DragDropEffects.None;
    }

    private (IList<DirectoryInfo> directories, IList<FileInfo> files) GetFoldersAndFilesFromPaths(IEnumerable<string> filePaths)
    {
        List<DirectoryInfo> directories = new();
        List<FileInfo> files = new();

        foreach (var path in filePaths)
        {
            if (File.Exists(path))
                files.Add(new FileInfo(path));
            else if (Directory.Exists(path))
                directories.Add(new DirectoryInfo(path));
        }

        return (directories, files);
    }
}
