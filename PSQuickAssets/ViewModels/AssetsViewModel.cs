using AsyncAwaitBestPractices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using PSQA.Core;
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
internal partial class AssetsViewModel
{
    public ObservableCollection<AssetGroupViewModel> AssetGroups { get; } = new();

    public Func<string, List<string>> IsGroupNameValid { get; }

    private readonly AssetRepository _assetRepository;
    private readonly INotificationService _notificationService;
    private readonly IStatusService _statusService;

    public AssetsViewModel(AssetRepository assetRepository, INotificationService notificationService, IStatusService statusService)
    {
        _assetRepository = assetRepository;
        _notificationService = notificationService;
        _statusService = statusService;

        AssetGroups = _assetRepository.AssetGroups;

        IsGroupNameValid = ValidateGroupName;

        //LoadStoredAssetsAsync().SafeFireAndForget(ex => _notificationService.Notify(
            //$"{Localization.Instance["Assets_FailedToLoadStoredAssetGroups"]} {ex.Message}", NotificationIcon.Error));
    }

    [ICommand]
    private void CreateEmptyGroup(string groupName)
    {
        if (string.IsNullOrWhiteSpace(groupName))
            groupName = GenerateNewGroupName();

        AssetGroup group = new(groupName);
        var vm = CreateGroupViewModel(group);
        AssetGroups.Add(vm);
        _assetRepository.Save();
    }

    [ICommand]
    private void RemoveGroup(AssetGroupViewModel? assetGroupViewModel)
    {
        if (assetGroupViewModel is not null && AssetGroups.Remove(assetGroupViewModel))
            _assetRepository.Save();
    }

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
            //await _assetGroupHandler.AddAssetsToGroupAsync(group, files);
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
            //await _assetGroupHandler.AddGroupFromFilesAsync(filePaths);
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
                //await _assetGroupHandler.AddGroupFromFolderAsync(path, includeSubfolders);
            }
        }
    }

    /// <summary>
    /// Checks if a group with specified name exists in <see cref="AssetGroups"/>.
    /// </summary>
    /// <returns><see langword="true"/> if group with that name exists.</returns>
    public bool IsGroupExists(string groupName)
    {
        return AssetGroups.Any(group => group.Name.Equals(groupName));
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

        if (IsGroupExists(name))
            errors.Add(Localization.Instance["Group_NameAlreadyExists"]);

        return errors;
    }

    /// <summary>
    /// Creates a group view model from existing <see cref="AssetGroup"/>.
    /// </summary>
    /// <returns>Created group viewmodel.</returns>
    private AssetGroupViewModel CreateGroupViewModel(AssetGroup assetGroup)
    {
        string groupName = assetGroup.Name;
        if (IsGroupExists(groupName))
        {
            groupName = $"{groupName} {Localization.Instance["New"]}";
            assetGroup.Name = groupName;
        }

        return new AssetGroupViewModel(assetGroup, _assetCatalogSaver);
    }

    /// <summary>
    /// Generates generic name for a new group.
    /// </summary>
    /// <returns>Generated name.</returns>
    private string GenerateNewGroupName()
    {
        string group = Localization.Instance["Group"];
        int genericGroupCount = AssetGroups.Select(g => g.Name.Contains(group, StringComparison.OrdinalIgnoreCase)).Count();
        return genericGroupCount == 0 ? group : group + " " + (genericGroupCount + 1);
    }
}