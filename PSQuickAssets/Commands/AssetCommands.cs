using CommunityToolkit.Mvvm.Input;
using MortuusUI.Controls;
using MortuusUI.Extensions;
using PSQA.Core;
using PSQuickAssets.ViewModels;
using System.Media;
using System.Windows;
using System.Windows.Input;

namespace PSQuickAssets.Commands;

internal static class AssetCommands
{
    public static ICommand ShowAssetInExplorer { get; } = new RelayCommand<object>((obj) =>
    {
        if (obj is Asset asset)
            GeneralCommands.ShowInExplorer.Execute(asset.Path);
        else
            SystemSounds.Exclamation.Play();
    });

    public static ICommand OpenAssetInShell { get; } = new RelayCommand<object>((obj) =>
    {
        if (obj is Asset asset)
            GeneralCommands.OpenInShell.Execute(asset.Path);
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
        if (element is null || element.GetDescendantByName<FrameworkElement>("GroupName") is not ToggleTextBox renameControl)
        {
            SystemSounds.Exclamation.Play();
            return;
        }

        renameControl.EditMode = true;
    });
}
