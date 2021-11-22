using Microsoft.Toolkit.Mvvm.ComponentModel;
using PSQuickAssets.Assets;
using PSQuickAssets.Models;
using PSQuickAssets.Services;
using PSQuickAssets.Utils.SystemDialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PSQuickAssets.ViewModels
{
    public class AssetsViewModel : ObservableObject
    {
        public ObservableCollection<AssetGroup> AssetGroups { get; }

        public bool IsLoading { get => isLoading; set { isLoading = value; OnPropertyChanged(nameof(IsLoading)); } }

        public ICommand AddFolderCommand { get; }
        public ICommand AddFolderWithSubfoldersCommand { get; }
        public ICommand AddFilesCommand { get; }
        public ICommand RemoveGroupCommand { get; }

        private readonly AssetLoader _assetLoader;
        private readonly AssetAtlas _assetAtlas;
        private readonly WindowManager _viewManager;

        private bool isLoading;

        internal AssetsViewModel(AssetLoader assetLoader, AssetAtlas assetAtlas, WindowManager viewManager)
        {
            AssetGroups = new ObservableCollection<AssetGroup>();

            _assetLoader = assetLoader;
            _assetAtlas = assetAtlas;
            _viewManager = viewManager;

            AddFolderCommand = new RelayCommand(_ => SelectAndAddFolders(includeSubfolders: false));
            AddFolderWithSubfoldersCommand = new RelayCommand(_ => SelectAndAddFolders(includeSubfolders: true));
            AddFilesCommand = new RelayCommand(_ => SelectAndAddFiles());
            RemoveGroupCommand = new RelayCommand(group => RemoveGroup((AssetGroup)group));

            AssetGroups.CollectionChanged += (_, _) => SaveAssets();

            LoadStoredAtlas();
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

        public async void SaveAssets()
        {
            await _assetAtlas.SaveAsync(AssetGroups);
        }

        public bool IsGroupExists(string groupName) => AssetGroups.Any(group => group.Name == groupName);

        public AssetGroup AddGroup(string groupName)
        {
            if (IsGroupExists(groupName))
                groupName = groupName + " New";

            AssetGroup assetGroup = new(groupName);
            assetGroup.GroupStateChanged += AssetGroup_OnGroupStateChanged;
            AssetGroups.Add(assetGroup);
            return assetGroup;
        }

        public bool RemoveGroup(AssetGroup assetGroup)
        {
            assetGroup.GroupStateChanged -= AssetGroup_OnGroupStateChanged;
            return AssetGroups.Remove(assetGroup);
        }

        private void AssetGroup_OnGroupStateChanged() => SaveAssets();

        private async void SelectAndAddFolders(bool includeSubfolders = false)
        {
            string[] folderPaths = SystemDialogs.SelectFolder("Select Folder", SelectionMode.Multiple);

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
            string[] files = SystemDialogs.SelectFiles("Select Assets", FileFilters.Images + "|" + FileFilters.AllFiles, SelectionMode.Multiple);

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
            group.AddMultipleAssets(assets);
        }

        private string GenericGroupName()
        {
            int genericGroupCount = AssetGroups.Select(g => g.Name.Contains("group", StringComparison.OrdinalIgnoreCase)).Count();
            return genericGroupCount == 0 ? "Group" : "Group " + (genericGroupCount + 1);
        }
    }
}
