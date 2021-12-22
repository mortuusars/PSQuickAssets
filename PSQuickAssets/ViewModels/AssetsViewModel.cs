using AsyncAwaitBestPractices;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using PSQuickAssets.Assets;
using PSQuickAssets.Configuration;
using PSQuickAssets.Models;
using PSQuickAssets.Resources;
using PSQuickAssets.Services;
using PSQuickAssets.Utils.SystemDialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PSQuickAssets.ViewModels
{
    public class AssetsViewModel : ObservableObject
    {
        public ObservableCollection<AssetGroup> AssetGroups { get; }

        public PhotoshopCommandsViewModel PhotoshopCommands { get; set; }

        public bool IsLoading { get => _isLoading; set { _isLoading = value; OnPropertyChanged(nameof(IsLoading)); } }

        public ICommand AddFolderCommand { get; }
        public ICommand AddFolderWithSubfoldersCommand { get; }
        public ICommand AddFilesCommand { get; }
        public ICommand RemoveGroupCommand { get; }

        private bool _isLoading;
        
        private readonly AssetLoader _assetLoader;
        private readonly AssetAtlas _assetAtlas;
        private readonly WindowManager _windowManager;
        private readonly INotificationService _notificationService;
        private readonly Config _config;

        internal AssetsViewModel(AssetLoader assetLoader, AssetAtlas assetAtlas, WindowManager windowManager, INotificationService notificationService, Config config)
        {
            AssetGroups = new ObservableCollection<AssetGroup>();
            PhotoshopCommands = new PhotoshopCommandsViewModel(windowManager, notificationService, config);

            _assetLoader = assetLoader;
            _assetAtlas = assetAtlas;
            _windowManager = windowManager;
            _notificationService = notificationService;
            _config = config;
            AddFolderCommand = new RelayCommand(() => SelectAndAddFolders(includeSubfolders: false));
            AddFolderWithSubfoldersCommand = new RelayCommand(() => SelectAndAddFolders(includeSubfolders: true));
            AddFilesCommand = new RelayCommand(SelectAndAddFiles);
            RemoveGroupCommand = new RelayCommand<AssetGroup>((group) => RemoveGroup(group!));

            AssetGroups.CollectionChanged += (_, _) => SaveAssets();

            LoadStoredAtlas();
        }

        public void SaveJson()
        {
            string json = JsonSerializer.Serialize(AssetGroups, new JsonSerializerOptions() { WriteIndented = true});
            var load = JsonSerializer.Deserialize<ObservableCollection<AssetGroup>>(json);
        }

        private async void LoadStoredAtlas()
        {
            IsLoading = true;

            StoredGroup[] groups = await _assetAtlas.LoadAsync();

            foreach (StoredGroup group in groups)
            {
                if (group.AssetsPaths is not null && group.AssetsPaths.Count > 0)
                {
                    var newGroup = AddGroup(group.Name);
                    await AddAssetsToGroup(newGroup, group.AssetsPaths);
                }
            }

            IsLoading = false;
        }

        public void SaveAssets()
        {
            _assetAtlas.SaveAsync(AssetGroups).SafeFireAndForget();
        }

        public bool IsGroupExists(string groupName) => AssetGroups.Any(group => group.Name == groupName);

        public AssetGroup AddGroup(string groupName)
        {
            if (IsGroupExists(groupName))
                groupName = $"{groupName} {Localization.Instance["New"]}";

            AssetGroup assetGroup = new(groupName);
            assetGroup.GroupChanged += AssetGroup_OnGroupStateChanged;
            AssetGroups.Add(assetGroup);
            return assetGroup;
        }

        public bool RemoveGroup(AssetGroup assetGroup)
        {
            assetGroup.GroupChanged -= AssetGroup_OnGroupStateChanged;
            return AssetGroups.Remove(assetGroup);
        }

        private void AssetGroup_OnGroupStateChanged() => SaveAssets();

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
        }

        private async void SelectAndAddFiles()
        {
            string[] files = SystemDialogs.SelectFiles(Localization.Instance["SelectAssets"], FileFilters.Images + "|" + FileFilters.AllFiles, SelectionMode.Multiple);

            if (files.Length == 0)
                return;

            IsLoading = true;
            await AddGroupFromFiles(files);
            IsLoading = false;
        }

        private async Task AddGroupFromFolder(string folderPath, bool includeSubfolders)
        {
            if (string.IsNullOrEmpty(folderPath))
                return;

            string[] files = Directory.GetFiles(folderPath);

            if (files.Length != 0)
            {
                AssetGroup group = AddGroup(new DirectoryInfo(folderPath).Name);
                await AddAssetsToGroup(group, files);
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

            var group = new AssetGroup(GenericGroupName());

            await AddAssetsToGroup(group, files);
            AssetGroups.Add(group);
        }

        private async Task AddAssetsToGroup(AssetGroup group, IEnumerable<string> files)
        {
            IEnumerable<Asset> assets = await Task.Run(() => _assetLoader.Load(files));
            group.AddMultipleAssets(assets, DuplicateHandling.Deny);
        }

        private string GenericGroupName()
        {
            string group = Localization.Instance["Group"];

            int genericGroupCount = AssetGroups.Select(g => g.Name.Contains(group, StringComparison.OrdinalIgnoreCase)).Count();
            return genericGroupCount == 0 ? group : group + " " + (genericGroupCount + 1);
        }
    }
}
