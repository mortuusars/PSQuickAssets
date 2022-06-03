using AsyncAwaitBestPractices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PSQA.Assets.Repository;
using PSQA.Core;
using PSQuickAssets.Models;
using PSQuickAssets.Services;
using System.Collections.Specialized;
using System.Windows.Input;

namespace PSQuickAssets.ViewModels;

[INotifyPropertyChanged]
internal partial class AssetsViewModel
{
    public ObservableCollection<AssetGroupViewModel> AssetGroups { get; } = new(); // Collection is managed by handling AssetRepository changes.

    [ObservableProperty]
    private bool _editMode;

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
    private void ToggleEditMode() => EditMode = !EditMode;

    [ICommand]
    private void ExitEditMode() => EditMode = false;

    [ICommand]
    private void AddEmptyGroup(string? groupName)
    {
        if (string.IsNullOrWhiteSpace(groupName) || IsGroupNameValid(groupName).Count > 0)
            groupName = GenerateGenericGroupName();

        _assetRepository.AssetGroups.Add(new AssetGroup() { Name = groupName });
    }

    [ICommand]
    private void RemoveGroup(AssetGroupViewModel? assetGroupViewModel)
    {
        if (assetGroupViewModel is not null)
            _assetRepository.AssetGroups.Remove(assetGroupViewModel.Group);
    }

    [ICommand]
    private void NewGroup(Func<IEnumerable<NewGroupData>> newGroupsProvider)
    {
        foreach (NewGroupData data in newGroupsProvider())
        {
            CreateGroup(data);
        }
    }

    private void CreateGroup(NewGroupData newGroupData)
    {
        string name = string.IsNullOrWhiteSpace(newGroupData.Name) ? GenerateGenericGroupName() : newGroupData.Name;
        if (IsGroupExists(name))
            name = GenerateGenericGroupName();

        List<Asset> assets = new();
        foreach (string path in newGroupData.FilePaths)
        {
            Asset asset = _assetRepository.CreateAsset(path);
            if (asset.Path != string.Empty)
                assets.Add(asset);
        }

        _assetRepository.AddGroup(name, assets);
    }

    [ICommand]
    private void GroupFromFiles(Func<IEnumerable<string>> filePathsProvider)
    {
        var files = filePathsProvider();
        if (!files.Any())
            return;

        List<Asset> assets = new();

        foreach (string path in files)
        {
            Asset asset = _assetRepository.CreateAsset(path);
            if (asset.Path != string.Empty)
                assets.Add(asset);
        }

        if (assets.Count > 0)
        {
            _assetRepository.AddGroup(GenerateGenericGroupName(), assets);
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
            errors.Add(Localize["Group_NameCannotBeEmpty"]);

        if (IsGroupExists(name))
            errors.Add(Localize["Group_NameAlreadyExists"]);

        return errors;
    }

    private AssetGroupViewModel CreateGroupViewModel(AssetGroup assetGroup)
    {
        return new AssetGroupViewModel(assetGroup, _assetRepository);
    }

    private string GenerateGenericGroupName()
    {
        string group = Localize["Group"];
        int genericGroupCount = AssetGroups.Select(g => g.Name.Contains(group, StringComparison.OrdinalIgnoreCase)).Count();
        return genericGroupCount == 0 ? group : group + " " + (genericGroupCount + 1);
    }
}