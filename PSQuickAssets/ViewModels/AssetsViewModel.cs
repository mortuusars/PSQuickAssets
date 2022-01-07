using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using MLogger;
using PSQuickAssets.Assets;
using PSQuickAssets.Commands;
using PSQuickAssets.Resources;
using PSQuickAssets.Services;
using PSQuickAssets.Utils.SystemDialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PSQuickAssets.ViewModels;

internal class AssetsViewModel : ObservableObject
{
    public ObservableCollection<AssetGroupViewModel> AssetGroups { get; private set; }
    public PhotoshopCommandsViewModel PhotoshopCommands { get; set; }
    public Func<string, bool> IsGroupNameValid { get; }
    public bool IsLoading { get => _isLoading; set { _isLoading = value; OnPropertyChanged(nameof(IsLoading)); } }

    public ICommand AddFolderCommand { get; }
    public ICommand AddFolderWithSubfoldersCommand { get; }
    public ICommand AddFilesCommand { get; }
    public ICommand RemoveGroupCommand { get; }

    public IAsyncCommand SaveGroupsAsyncCommand { get; }

    private bool _isLoading;

    private readonly AssetManager _assetManager;
    private readonly INotificationService _notificationService;
    private readonly ILogger _logger;

    public AssetsViewModel(AssetManager assetManager, PhotoshopCommandsViewModel photoshopCommandsViewModel, INotificationService notificationService, ILogger logger)
    {
        AssetGroups = new ObservableCollection<AssetGroupViewModel>();
        PhotoshopCommands = photoshopCommandsViewModel;

        _assetManager = assetManager;
        _notificationService = notificationService;
        _logger = logger;

        IsGroupNameValid = new Func<string, bool>((s) => !string.IsNullOrWhiteSpace(s) && !IsGroupExists(s));
        AddFolderCommand = new RelayCommand(() => SelectAndAddFolders(includeSubfolders: false));
        AddFolderWithSubfoldersCommand = new RelayCommand(() => SelectAndAddFolders(includeSubfolders: true));
        AddFilesCommand = new RelayCommand(SelectAndAddFiles);
        RemoveGroupCommand = new RelayCommand<AssetGroupViewModel>(g => RemoveGroup(g));

        SaveGroupsAsyncCommand = new SaveGroupsAsyncCommand(this, assetManager, notificationService);

        LoadStoredGroupsAsync().SafeFireAndForget(ex => _notificationService.Notify("Failed to load saved asset groups: " + ex.Message, NotificationIcon.Error));
    }

    /// <summary>
    /// Asynchronously loads stored asset groups and adds them to <see cref="AssetGroups"/> one by one.
    /// </summary>
    private async Task LoadStoredGroupsAsync()
    {
        var populatedCollection = new ObservableCollection<AssetGroup>();
        populatedCollection.CollectionChanged += (s, e) =>
        {
            if (e.NewItems is null)
                return;

            foreach (var item in e.NewItems)
            {
                AssetGroup assetGroup = (AssetGroup)item;
                CreateGroup(assetGroup);
            }
        };

        IsLoading = true;
        await _assetManager.LoadGroupsToCollectionAsync(populatedCollection);
        IsLoading = false;
    }

    /// <summary>
    /// Checks if a group with specified name exists in <see cref="AssetGroups"/>.
    /// </summary>
    /// <returns><see langword="true"/> if group with that name exists.</returns>
    private bool IsGroupExists(string groupName) => AssetGroups.Any(group => group.Name.Equals(groupName));

    /// <summary>
    /// Removes group from <see cref="AssetGroups"/>.
    /// </summary>
    /// <param name="assetGroupViewModel">Group to remove.</param>
    /// <returns><see langword="true"/> if group was removed.</returns>
    private bool RemoveGroup(AssetGroupViewModel? assetGroupViewModel)
    {
        bool isRemoved = assetGroupViewModel is not null && AssetGroups.Remove(assetGroupViewModel);
        if (isRemoved)
        {
            _logger.Info($"[Asset Groups] Removed group '{assetGroupViewModel!.Name}'.");
            SaveGroupsAsyncCommand.ExecuteAsync().SafeFireAndForget();
        }
        return isRemoved;
    }

    private async void SelectAndAddFolders(bool includeSubfolders = false)
    {
        string[] folderPaths = SystemDialogs.SelectFolder(Localization.Instance["SelectFolder"], SelectionMode.Multiple);

        if (folderPaths.Length == 0)
            return;

        IsLoading = true;
        foreach (var path in folderPaths)
        {
            await AddGroupFromFolder(path, includeSubfolders);
        }
        IsLoading = false;
        SaveGroupsAsyncCommand.ExecuteAsync().SafeFireAndForget();
    }

    private async void SelectAndAddFiles()
    {
        string[] files = SystemDialogs.SelectFiles(Localization.Instance["SelectAssets"], FileFilters.Images + "|" + FileFilters.AllFiles, SelectionMode.Multiple);

        if (files.Length == 0)
            return;

        IsLoading = true;
        await AddGroupFromFiles(files);
        IsLoading = false;
        SaveGroupsAsyncCommand.ExecuteAsync().SafeFireAndForget();
    }

    private async Task AddGroupFromFolder(string folderPath, bool includeSubfolders)
    {
        if (string.IsNullOrEmpty(folderPath))
            return;

        string[] files = Directory.GetFiles(folderPath);

        if (files.Length != 0)
        {
            AssetGroupViewModel group = CreateEmptyGroup(new DirectoryInfo(folderPath).Name);
            await AddAssetsToGroup(group, files);

            if (group.Group.Assets.Count == 0)
                RemoveGroup(group);
        }

        if (includeSubfolders)
        {
            foreach (var folder in Directory.GetDirectories(folderPath))
                await AddGroupFromFolder(folder, includeSubfolders);
        }
    }

    private async Task AddGroupFromFiles(IList<string> files)
    {
        if (files.Count == 0)
            return;

        var group = CreateEmptyGroup();

        await AddAssetsToGroup(group, files);
    }

    private async Task AddAssetsToGroup(AssetGroupViewModel group, IEnumerable<string> files)
    {
        IsLoading = true;
        var assets = await Task.Run(() => _assetManager.Load(files));
        group.AddAssets(assets, DuplicateHandling.Deny);
        IsLoading = false;
    }


    /// <summary>
    /// Creates empty group with specified name. Group will be added to <see cref="AssetGroups"/>.
    /// </summary>
    /// <param name="groupName">Name for a group.</param>
    /// <returns>Created group.</returns>
    private AssetGroupViewModel CreateEmptyGroup(string groupName)
    {
        AssetGroup group = new(groupName);
        return CreateGroup(group);
    }
    /// <summary>
    /// Creates empty group with a generic name. Group will be added to <see cref="AssetGroups"/>.
    /// </summary>
    /// <returns>Created group.</returns>
    private AssetGroupViewModel CreateEmptyGroup() => CreateEmptyGroup(GenericGroupName());
    /// <summary>
    /// Creates empty group from existing <see cref="AssetGroup"/>. Group will be added to <see cref="AssetGroups"/>.
    /// </summary>
    /// <returns>Created group.</returns>
    private AssetGroupViewModel CreateGroup(AssetGroup assetGroup)
    {
        string groupName = assetGroup.Name;
        if (IsGroupExists(groupName))
        {
            groupName = $"{groupName} {Localization.Instance["New"]}";
            assetGroup.Name = groupName;
        }

        var groupViewModel = new AssetGroupViewModel(assetGroup, _logger);
        groupViewModel.PropertyChanged += (s, e) => SaveGroupsAsyncCommand.ExecuteAsync().SafeFireAndForget();
        AssetGroups.Add(groupViewModel);
        _logger.Info($"[Asset Groups] Group '{groupName}' created.");
        return groupViewModel;
    }
    /// <summary>
    /// Generates generic name for a new group. 
    /// </summary>
    /// <returns>Generated name.</returns>
    private string GenericGroupName()
    {
        string group = Localization.Instance["Group"];
        int genericGroupCount = AssetGroups.Select(g => g.Name.Contains(group, StringComparison.OrdinalIgnoreCase)).Count();
        return genericGroupCount == 0 ? group : group + " " + (genericGroupCount + 1);
    }
}