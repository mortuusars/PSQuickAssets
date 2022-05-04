using CommunityToolkit.Mvvm.Input;
using PSQA.Core;
using PSQuickAssets.ViewModels;
using System.Media;
using PureUI;
using System.Windows;
using System.Windows.Input;
using PureUI.Controls;

namespace PSQuickAssets.Commands;

internal static class AssetCommands
{
    public static ICommand ShowAssetInExplorer { get; } = new RelayCommand<object>((obj) =>
    {
        if (obj is AssetViewModel asset)
            GeneralCommands.ShowInExplorer.Execute(asset.FilePath);
        else
            SystemSounds.Exclamation.Play();
    });

    public static ICommand OpenAssetInShell { get; } = new RelayCommand<object>((obj) =>
    {
        if (obj is AssetViewModel asset)
            GeneralCommands.OpenInShell.Execute(asset.FilePath);
        else
            SystemSounds.Exclamation.Play();
    });

    public static ICommand CollapseExpandGroup { get; } = new RelayCommand<object>((groupViewModel) =>
    {
        if (groupViewModel is AssetGroupViewModel groupVM)
            groupVM.IsExpanded = !groupVM.IsExpanded;
        else
            SystemSounds.Asterisk.Play();
    });

    public static ICommand RenameGroup { get; } = new RelayCommand<FrameworkElement>((element) =>
    {
        if (element?.FindChildByName("GroupName") is ToggleTextBox renameControl)
            renameControl.EditMode = true;
        else
            SystemSounds.Exclamation.Play();
    });
}
