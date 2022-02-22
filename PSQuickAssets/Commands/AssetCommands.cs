using CommunityToolkit.Mvvm.Input;
using MortuusUI.Controls;
using MortuusUI.Extensions;
using PSQuickAssets.Assets;
using PSQuickAssets.ViewModels;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PSQuickAssets.Commands;

internal static class AssetCommands
{
    public static ICommand ShowAssetInExplorer { get; } = new RelayCommand<Asset>((a) =>
    {
        if (a is null)
        {
            SystemSounds.Exclamation.Play();
            return;
        }

        GeneralCommands.ShowInExplorer.Execute(a.Path);
    });

    public static ICommand OpenAssetInShell { get; } = new RelayCommand<Asset>((a) =>
    {
        if (a is null)
        {
            SystemSounds.Exclamation.Play();
            return;
        }

        GeneralCommands.OpenInShell.Execute(a.Path);
    });

    public static ICommand CollapseExpandGroup { get; } = new RelayCommand<object>((groupViewModel) =>
    {
        if (groupViewModel is AssetGroupViewModel groupVM)
            groupVM.IsExpanded = !groupVM.IsExpanded;
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
