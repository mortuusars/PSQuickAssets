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
    public ObservableCollection<AssetGroupViewModel> AssetGroups { get; } = new();

    public Func<string, List<string>> IsGroupNameValid { get; }

    private readonly AssetGroupHandler _assetGroupHandler;
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
        _assetGroupHandler = new AssetGroupHandler(AssetGroups, assetManager, logger);

        IsGroupNameValid = IsNameForAGroupValid;

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

    private async Task SaveGroupsAsync()
    {
        var result = await _assetGroupHandler.SaveGroupsAsync();

        if (result.IsSuccessful)
            return;

        string errorMessage = Localization.Instance["FailedToSaveGroups"];

        foreach (var group in result.FailedGroups)
        {
            errorMessage += $"\n{group.Key.Name} : {group.Value.Message}";
        }

        _notificationService.Notify(errorMessage, NotificationIcon.Error,
            () => System.Windows.MessageBox.Show(errorMessage, App.AppName, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error));
    }

    [ICommand]
    private void CreateEmptyGroup(string groupName)
    {
        _assetGroupHandler.CreateEmptyGroup(groupName);
        SaveGroupsAsync().SafeFireAndForget();
    }

    [ICommand]
    private void RemoveGroup(AssetGroupViewModel? assetGroupViewModel)
    {
        _assetGroupHandler.RemoveGroup(assetGroupViewModel);
        SaveGroupsAsync().SafeFireAndForget();
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
            await _assetGroupHandler.AddAssetsToGroupAsync(group, files);
        }
        SaveGroupsAsync().SafeFireAndForget();
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
        SaveGroupsAsync().SafeFireAndForget();
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
        SaveGroupsAsync().SafeFireAndForget();
    }

    private List<string> IsNameForAGroupValid(string name)
    {
        List<string> errors = new();

        if (string.IsNullOrWhiteSpace(name))
            errors.Add(Localization.Instance["Group_NameCannotBeEmpty"]);

        if (_assetGroupHandler.IsGroupExists(name))
            errors.Add(Localization.Instance["Group_NameAlreadyExists"]);

        return errors;
    }
}