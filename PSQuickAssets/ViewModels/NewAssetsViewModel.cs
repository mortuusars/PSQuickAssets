using AsyncAwaitBestPractices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PSQuickAssets.Assets;
using PSQuickAssets.Resources;
using PSQuickAssets.Services;
using PSQuickAssets.Utils.SystemDialogs;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSQuickAssets.ViewModels;

//[INotifyPropertyChanged]
internal partial class NewAssetsViewModel
{
    public ObservableCollection<AssetGroupViewModel> AssetGroups { get; }

    public Func<string, List<string>> IsGroupNameValid { get; }

    private readonly AssetGroupHandler _assetGroupHandler;
    private readonly INotificationService _notificationService;
    private readonly IStatusService _statusService;

    public NewAssetsViewModel(ObservableCollection<AssetGroupViewModel> assetGroups, 
        AssetGroupHandler assetGroupHandler, INotificationService notificationService, IStatusService statusService)
    {
        AssetGroups = assetGroups;
        _assetGroupHandler = assetGroupHandler;
        _notificationService = notificationService;
        _statusService = statusService;

        IsGroupNameValid = ValidateGroupName;

        LoadStoredAssetsAsync().SafeFireAndForget(ex => _notificationService.Notify(
            $"{Localization.Instance["Assets_FailedToLoadStoredAssetGroups"]} {ex.Message}", NotificationIcon.Error));
    }

    private async Task LoadStoredAssetsAsync()
    {
        using (_statusService.Loading(Localization.Instance["Assets_LoadingAssets"]))
        {
            await _assetGroupHandler.LoadStoredGroupsAsync();
        }
    }

    [ICommand]
    private void CreateEmptyGroup(string groupName) => _assetGroupHandler.CreateEmptyGroup(groupName);

    [ICommand]
    private void RemoveGroup(AssetGroupViewModel? assetGroupViewModel) => _assetGroupHandler.RemoveGroup(assetGroupViewModel);

    [ICommand]
    private async Task AddFilesToGroup(AssetGroupViewModel? group)
    {
        if (group is null)
            throw new ArgumentNullException(nameof(group));

        string[] files = SystemDialogs.SelectFiles(Localization.Instance["SelectAssets"], FileFilters.Images + "|" + FileFilters.AllFiles, SelectionMode.Multiple);

        if (files.Length == 0)
            return;

        using (_statusService.Loading(Localization.Instance["Assets_AddingAssets"]))
        {
            await _assetGroupHandler.AddAssetsToGroupAsync(group, files);
        }
    }

    [ICommand]
    private async Task NewGroupFromFiles()
    {
        string[] filePaths = SystemDialogs.SelectFiles(Localization.Instance["SelectAssets"],
            FileFilters.Images + "|" + FileFilters.AllFiles, SelectionMode.Multiple);

        if (filePaths.Length == 0)
            return;

        using (_statusService.Loading(Localization.Instance["Assets_AddingAssets"]))
        {
            await _assetGroupHandler.AddGroupFromFilesAsync(filePaths);
        }
    }

    [ICommand]
    private async void NewGroupFromFolder(bool includeSubfolders = false)
    {
        string[] folderPaths = SystemDialogs.SelectFolder(Localization.Instance["SelectFolder"], SelectionMode.Multiple);

        if (folderPaths.Length == 0)
            return;

        using (_statusService.Loading(Localization.Instance["Assets_AddingAssets"]))
        {
            foreach (var path in folderPaths)
            {
                await _assetGroupHandler.AddGroupFromFolderAsync(path, includeSubfolders);
            }
        }
    }

    /// <summary>
    /// Validates new name for a group.
    /// </summary>
    /// <returns>List of errors if not valid. Empty list if name is valid.</returns>
    private List<string> ValidateGroupName(string name)
    {
        List<string> errors = new();

        if (string.IsNullOrWhiteSpace(name))
            errors.Add(Localization.Instance["Group_NameCannotBeEmpty"]);

        if (_assetGroupHandler.IsGroupExists(name))
            errors.Add(Localization.Instance["Group_NameAlreadyExists"]);

        return errors;
    }
}