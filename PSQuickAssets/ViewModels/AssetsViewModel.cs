using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
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

    public AssetsViewModel(AssetManager assetManager, PhotoshopCommandsViewModel photoshopCommandsViewModel, INotificationService notificationService)
    {
        AssetGroups = new ObservableCollection<AssetGroupViewModel>();
        PhotoshopCommands = photoshopCommandsViewModel;

        _assetManager = assetManager;
        _notificationService = notificationService;

        IsGroupNameValid = new Func<string, bool>((s) => !string.IsNullOrWhiteSpace(s) && !IsGroupExists(s));
        AddFolderCommand = new RelayCommand(() => SelectAndAddFolders(includeSubfolders: false));
        AddFolderWithSubfoldersCommand = new RelayCommand(() => SelectAndAddFolders(includeSubfolders: true));
        AddFilesCommand = new RelayCommand(SelectAndAddFiles);
        RemoveGroupCommand = new RelayCommand<AssetGroupViewModel>(g => RemoveGroup(g));

        SaveGroupsAsyncCommand = new SaveGroupsAsyncCommand(this, assetManager, notificationService);

        LoadStoredGroupsAsync().SafeFireAndForget();
    }

    public bool IsGroupExists(string groupName) => AssetGroups.Any(group => group.Name.Equals(groupName));

    public AssetGroup CreateGroup(string groupName)
    {
        if (IsGroupExists(groupName))
            groupName = $"{groupName} {Localization.Instance["New"]}";

        AssetGroup assetGroup = new(groupName);
        AssetGroupViewModel groupVM = new AssetGroupViewModel(assetGroup);
        AssetGroups.Add(groupVM);
        groupVM.PropertyChanged += (s, e) => SaveGroupsAsyncCommand.ExecuteAsync().SafeFireAndForget();
        return assetGroup;
    }

    public bool RemoveGroup(AssetGroupViewModel? assetGroupViewModel)
    {
        bool isRemoved = assetGroupViewModel is not null && AssetGroups.Remove(assetGroupViewModel);
        if (isRemoved)
            SaveGroupsAsyncCommand.ExecuteAsync().SafeFireAndForget();
        return isRemoved;
    }

    private async Task LoadStoredGroupsAsync()
    {
        IsLoading = true;

        var observableColl = new ObservableCollection<AssetGroup>();
        observableColl.CollectionChanged += (s, e) =>
        {
            if (e.NewItems is null)
                return;

            foreach (var item in e.NewItems)
            {
                AssetGroups.Add(new AssetGroupViewModel((AssetGroup)item));
            }
        };

        await _assetManager.LoadGroupsToCollectionAsync(observableColl);
        IsLoading = false;
     
        foreach (var group in AssetGroups)
            group.PropertyChanged += (s, e) => SaveGroupsAsyncCommand.ExecuteAsync().SafeFireAndForget();
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
            AssetGroup group = CreateGroup(new DirectoryInfo(folderPath).Name);
            var groupVM = new AssetGroupViewModel(group);
            await AddAssetsToGroup(groupVM, files);
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

        var group = CreateGroup(GenericGroupName());
        var groupVM = new AssetGroupViewModel(group);

        await AddAssetsToGroup(groupVM, files);
        AssetGroups.Add(new AssetGroupViewModel(group));
    }

    private async Task AddAssetsToGroup(AssetGroupViewModel group, IEnumerable<string> files)
    {
        var assets = await Task.Run(() => _assetManager.Load(files));
        group.AddAssets(assets, DuplicateHandling.Deny);
    }

    private string GenericGroupName()
    {
        string group = Localization.Instance["Group"];
        int genericGroupCount = AssetGroups.Select(g => g.Name.Contains(group, StringComparison.OrdinalIgnoreCase)).Count();
        return genericGroupCount == 0 ? group : group + " " + (genericGroupCount + 1);
    }
}