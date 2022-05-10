using CommunityToolkit.Mvvm.Input;
using PSQuickAssets.ViewModels;
using PureUI;
using PureUI.Controls;
using System.Media;
using System.Windows;
using System.Windows.Input;

namespace PSQuickAssets.Commands;

internal static class AssetCommands
{
    public static ICommand CopyAssetFilePath { get; } = new RelayCommand<object>(obj =>
    {
        if (obj is AssetViewModel asset)
            Clipboard.SetText(asset.FilePath);
        else
            SystemSounds.Exclamation.Play();
    });

    public static ICommand ShowAssetInExplorer { get; } = new RelayCommand<object>(obj =>
    {
        if (obj is AssetViewModel asset)
            GeneralCommands.ShowInExplorer.Execute(asset.FilePath);
        else
            SystemSounds.Exclamation.Play();
    });

    public static ICommand OpenAssetInShell { get; } = new RelayCommand<object>(obj =>
    {
        if (obj is AssetViewModel asset)
            GeneralCommands.OpenInShell.Execute(asset.FilePath);
        else
            SystemSounds.Exclamation.Play();
    });

    public static ICommand CollapseExpandGroup { get; } = new RelayCommand<object>(groupViewModel =>
    {
        if (groupViewModel is AssetGroupViewModel groupVM)
            groupVM.IsExpanded = !groupVM.IsExpanded;
        else
            SystemSounds.Asterisk.Play();
    });

    public static ICommand RenameGroup { get; } = new RelayCommand<FrameworkElement>(element =>
    {
        if (element?.FindChildByName("GroupName") is ToggleTextBox renameControl)
            renameControl.EditMode = true;
        else
            SystemSounds.Exclamation.Play();
    });
}
