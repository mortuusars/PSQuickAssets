﻿using AsyncAwaitBestPractices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PSQA.Assets.Repository;
using PSQA.Core;
using PSQuickAssets.Resources;
using PSQuickAssets.Services;
using PSQuickAssets.Utils.SystemDialogs;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PSQuickAssets.ViewModels;

[INotifyPropertyChanged]
internal partial class AssetsViewModel
{
    // This collection is managed by handling AssetRepository changes.
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

        _assetRepository.Load();
        AddAllGroupsFromRepository();
        _assetRepository.AssetGroups.CollectionChanged += OnRepositoryGroupsChanged;

        IsGroupNameValid = ValidateGroupName;
    }

    private void OnRepositoryGroupsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                {
                    if (e.NewItems is not null)
                        foreach (AssetGroup group in e.NewItems)
                            AssetGroups.Add(CreateGroupViewModel(group));
                }
                break;
            case NotifyCollectionChangedAction.Remove:
                {
                    if (e.OldItems is not null)
                        foreach (AssetGroup group in e.OldItems)
                            if (AssetGroups.FirstOrDefault(g => g.Group == group) is AssetGroupViewModel vm)
                                AssetGroups.Remove(vm);
                }
                break;
            case NotifyCollectionChangedAction.Replace:
                {
                    if (e.OldItems is null || e.NewItems is null)
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
            case NotifyCollectionChangedAction.Reset:
                {
                    AssetGroups.Clear();
                    AddAllGroupsFromRepository();
                }
                break;
        }

        _assetRepository.SaveAsync().SafeFireAndForget();
    }

    private void AddAllGroupsFromRepository()
    {
        foreach (AssetGroup group in _assetRepository.AssetGroups)
            AssetGroups.Add(CreateGroupViewModel(group));
    }

    [ICommand]
    private void CreateEmptyGroup(string? groupName)
    {
        if (string.IsNullOrWhiteSpace(groupName))
            groupName = GenerateNewGroupName();

        _assetRepository.AssetGroups.Add(new AssetGroup() { Name = groupName });
    }

    [ICommand]
    private void RemoveGroup(AssetGroupViewModel? assetGroupViewModel)
    {
        if (assetGroupViewModel is not null)
            _assetRepository.AssetGroups.Remove(assetGroupViewModel.Group);
    }

    //[ICommand]
    //private async Task AddFilesToGroup(AssetGroupViewModel? group)
    //{
    //    if (group is null)
    //        throw new ArgumentNullException(nameof(group));

    //    string[] files = SystemDialogs.SelectFiles(Localization.Instance["SelectAssets"], FileFilters.Images + "|" + FileFilters.AllFiles, SelectionMode.Multiple);

    //    if (files.Length == 0)
    //        return;

    //    using (_statusService.Loading(Localization.Instance["Assets_AddingAssets"]))
    //    {
    //        //await _assetGroupHandler.AddAssetsToGroupAsync(group, files);
    //    }
    //}

    //[ICommand]
    //private async Task NewGroupFromFiles()
    //{
    //    string[] filePaths = SystemDialogs.SelectFiles(Localize[nameof(Lang.SelectImages)],
    //        FileFilters.Images + "|" + FileFilters.AllFiles, SelectionMode.Multiple);

    //    if (filePaths.Length == 0)
    //        return;

    //    using (_statusService.Loading(Localization.Instance["Assets_AddingAssets"]))
    //    {
    //        //await _assetGroupHandler.AddGroupFromFilesAsync(filePaths);
    //    }
    //}

    //[ICommand]
    //private async void NewGroupFromFolder(bool includeSubfolders = false)
    //{
    //    string[] folderPaths = SystemDialogs.SelectFolder(Localization.Instance["SelectFolder"], SelectionMode.Multiple);

    //    if (folderPaths.Length == 0)
    //        return;

    //    using (_statusService.Loading(Localization.Instance["Assets_AddingAssets"]))
    //    {
    //        foreach (var path in folderPaths)
    //        {
    //            //await _assetGroupHandler.AddGroupFromFolderAsync(path, includeSubfolders);
    //        }
    //    }
    //}

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
            errors.Add(Localize["Group_NameCannotBeEmpty"]);

        if (IsGroupExists(name))
            errors.Add(Localize["Group_NameAlreadyExists"]);

        return errors;
    }

    /// <summary>
    /// Creates a group view model from existing <see cref="AssetGroup"/>.
    /// </summary>
    /// <returns>Created group viewmodel.</returns>
    private AssetGroupViewModel CreateGroupViewModel(AssetGroup assetGroup)
    {
        return new AssetGroupViewModel(assetGroup, _assetRepository);
    }

    /// <summary>
    /// Generates generic name for a new group.
    /// </summary>
    /// <returns>Generated name.</returns>
    private string GenerateNewGroupName()
    {
        string group = Localize["Group"];
        int genericGroupCount = AssetGroups.Select(g => g.Name.Contains(group, StringComparison.OrdinalIgnoreCase)).Count();
        return genericGroupCount == 0 ? group : group + " " + (genericGroupCount + 1);
    }
}