using AsyncAwaitBestPractices;
using CommunityToolkit.Mvvm.Input;
using PSQA.Assets.Repository;
using PSQA.Core;
using PSQuickAssets.Resources;
using PSQuickAssets.Services;
using PSQuickAssets.Utils.SystemDialogs;
using PureLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
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
        _assetRepository.AssetGroups.CollectionChanged += OnRepositoryGroupsChanged;
        _notificationService = notificationService;
        _statusService = statusService;

        _assetRepository.Load();

        IsGroupNameValid = ValidateGroupName;
    }

    private void OnRepositoryGroupsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems is null)
            return;

        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                {
                    foreach (AssetGroup group in e.NewItems)
                        AssetGroups.Add(CreateGroupViewModel(group));
                }
                break;
            case NotifyCollectionChangedAction.Remove:
                {
                    foreach (AssetGroup group in e.NewItems)
                        RemoveGroup(AssetGroups.FirstOrDefault(g => g.Group == group));
                }
                break;
            case NotifyCollectionChangedAction.Replace:
                {
                    if (e.OldItems is null)
                        return;

                    for (int i = 0; i < e.OldItems.Count; i++)
                    {
                        if (e.NewItems[i]?.CastTo<AssetGroup>() is not AssetGroup oldGroup)
                            continue;
                        if (AssetGroups.FirstOrDefault(g => g.Group == oldGroup) is not AssetGroupViewModel oldVM)
                            continue;
                        if (e.OldItems[i]?.CastTo<AssetGroup>() is not AssetGroup newGroup)
                            continue;

                        AssetGroups[AssetGroups.IndexOf(oldVM)] = CreateGroupViewModel(newGroup);
                    }
                }
                break;
            case NotifyCollectionChangedAction.Move:
                {
                    AssetGroups.Clear();
                    foreach (AssetGroup group in _assetRepository.AssetGroups)
                        AssetGroups.Add(CreateGroupViewModel(group));
                }
                break;
            case NotifyCollectionChangedAction.Reset:
                {
                    AssetGroups.Clear();
                    foreach (AssetGroup group in _assetRepository.AssetGroups)
                        AssetGroups.Add(CreateGroupViewModel(group));
                }
                break;
        }

        _assetRepository.SaveAsync().SafeFireAndForget();
    }

    [ICommand]
    private void CreateEmptyGroup(string? groupName)
    {
        if (string.IsNullOrWhiteSpace(groupName))
            groupName = GenerateNewGroupName();

        _assetRepository.AssetGroups.Add(new AssetGroup(groupName));
    }

    [ICommand]
    private void RemoveGroup(AssetGroupViewModel? assetGroupViewModel)
    {
        if (assetGroupViewModel is not null)
            _assetRepository.AssetGroups.Remove(assetGroupViewModel.Group);
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

        var newViewModel = new AssetGroupViewModel(assetGroup);
        newViewModel.GroupChanged += (s, e) => _assetRepository.SaveAsync().SafeFireAndForget();

        return newViewModel;
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