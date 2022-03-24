using AsyncAwaitBestPractices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PSQuickAssets.Assets;
using PSQuickAssets.Resources;
using PSQuickAssets.Services;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSQuickAssets.ViewModels;

[INotifyPropertyChanged]
internal partial class NewAssetsViewModel
{
    public ObservableCollection<AssetGroupViewModel> AssetGroups { get; } = new();

    private readonly AssetManager _assetManager;
    private readonly INotificationService _notificationService;
    private readonly IStatusService _statusService;
    private readonly ILogger _logger;

    public NewAssetsViewModel(AssetManager assetManager, INotificationService notificationService, IStatusService statusService, ILogger logger)
    {
        _assetManager = assetManager;
        _notificationService = notificationService;
        _statusService = statusService;
        _logger = logger;

        LoadStoredAssetsAsync().SafeFireAndForget(ex => _notificationService.Notify(
            Localization.Instance["Assets_FailedToLoadStoredGroups"] + " " + ex.Message, NotificationIcon.Error));
    }

    private async Task LoadStoredAssetsAsync()
    {
        using (_statusService.LoadingStatus())
        {
            await foreach (var group in _assetManager.LoadStoredGroupsAsync())
            {
                if (group is not null)
                    AssetGroups.Add(CreateGroupViewModel(group));
            }
        }
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

        _logger.Information($"[Asset Groups] Group '{groupName}' created.");
        return new AssetGroupViewModel(assetGroup, _logger);
    }

    /// <summary>
    /// Removes group from <see cref="AssetGroups"/>.
    /// </summary>
    /// <param name="assetGroupViewModel">Group to remove.</param>
    /// <returns><see langword="true"/> if group was removed.</returns>
    [ICommand]
    private void RemoveGroup(AssetGroupViewModel? assetGroupViewModel)
    {
        if (assetGroupViewModel is not null && AssetGroups.Remove(assetGroupViewModel))
            _logger.Information($"[Asset Groups] Removed group '{assetGroupViewModel!.Name}'.");
    }

    /// <summary>
    /// Checks if a group with specified name exists in <see cref="AssetGroups"/>.
    /// </summary>
    /// <returns><see langword="true"/> if group with that name exists.</returns>
    private bool IsGroupExists(string groupName) => AssetGroups.Any(group => group.Name.Equals(groupName));
}
