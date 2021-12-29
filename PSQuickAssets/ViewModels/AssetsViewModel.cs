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
        public ObservableCollection<AssetGroup> AssetGroups { get; private set; }

        public PhotoshopCommandsViewModel PhotoshopCommands { get; set; }

        public bool IsLoading { get => _isLoading; set { _isLoading = value; OnPropertyChanged(nameof(IsLoading)); } }

        public ICommand AddFolderCommand { get; }
        public ICommand AddFolderWithSubfoldersCommand { get; }
        public ICommand AddFilesCommand { get; }
        public ICommand RemoveGroupCommand { get; }
        public ICommand ToggleGroupExpandCommand { get; }

        private bool _isLoading;

        private readonly AssetManager _assetManager;
        private readonly WindowManager _windowManager;
        private readonly INotificationService _notificationService;
        private readonly Config _config;

        internal AssetsViewModel(AssetManager assetManager, WindowManager windowManager, INotificationService notificationService, Config config)
        {
            AssetGroups = new ObservableCollection<AssetGroup>();
            PhotoshopCommands = new PhotoshopCommandsViewModel(windowManager, notificationService, config);

            _assetManager = assetManager;
            _windowManager = windowManager;
            _notificationService = notificationService;
            _config = config;

            AddFolderCommand = new RelayCommand(() => SelectAndAddFolders(includeSubfolders: false));
            AddFolderWithSubfoldersCommand = new RelayCommand(() => SelectAndAddFolders(includeSubfolders: true));
            AddFilesCommand = new RelayCommand(SelectAndAddFiles);
            RemoveGroupCommand = new RelayCommand<AssetGroup>((group) => RemoveGroup(group!));

            ToggleGroupExpandCommand = new RelayCommand<AssetGroup>((group) => ToggleGroupExpanded(group));

            LoadStoredGroupsAsync().SafeFireAndForget();
        }

        public async Task SaveGroupsAsync()
        {
            Result saveResult = await _assetManager.SaveAsync(AssetGroups);

            if (saveResult.IsSuccessful)
                return;

            if (saveResult.Exception is AggregateException aggregateException)
            {
                if (aggregateException.InnerExceptions.Count == AssetGroups.Count)
                    _notificationService.Notify(App.AppName, Localization.Instance["FailedToSaveAllGroups"], NotificationIcon.Error);
                else
                {
                    string failedGroups = "";
                    foreach (var exception in aggregateException.InnerExceptions)
                    {
                        failedGroups += exception.Message;
                    }

                    _notificationService.Notify(App.AppName, Localization.Instance["FailedToSaveGroups"] + $"\n<{failedGroups}>", NotificationIcon.Error);
                }
            }
            else
                _notificationService.Notify(App.AppName, Localization.Instance["FailedToSaveAssetGroups"] + saveResult.Exception?.Message, NotificationIcon.Error);
        }

        public bool IsGroupExists(string groupName) => AssetGroups.Any(group => group.Name == groupName);

        public AssetGroup AddGroup(string groupName)
        {
            if (IsGroupExists(groupName))
                groupName = $"{groupName} {Localization.Instance["New"]}";

            AssetGroup assetGroup = new(groupName);
            AssetGroups.Add(assetGroup);
            SaveGroupsAsync().SafeFireAndForget();
            return assetGroup;
        }

        public bool RemoveGroup(AssetGroup assetGroup)
        {
            bool isRemoved = AssetGroups.Remove(assetGroup);
            if (isRemoved)
                SaveGroupsAsync().SafeFireAndForget();
            return isRemoved;
        }

        private void ToggleGroupExpanded(AssetGroup? group)
        {
            if (group is null)
                throw new ArgumentNullException(nameof(group), "Group cannot be null");

            group.IsExpanded = !group.IsExpanded;
            SaveGroupsAsync().SafeFireAndForget();
        }

        private async Task LoadStoredGroupsAsync()
        {
            IsLoading = true;
            await _assetManager.LoadGroupsToCollectionAsync(AssetGroups);
            IsLoading = false;
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
            SaveGroupsAsync().SafeFireAndForget();
            IsLoading = false;
        }

        private async void SelectAndAddFiles()
        {
            string[] files = SystemDialogs.SelectFiles(Localization.Instance["SelectAssets"], FileFilters.Images + "|" + FileFilters.AllFiles, SelectionMode.Multiple);

            if (files.Length == 0)
                return;

            IsLoading = true;
            await AddGroupFromFiles(files);
            SaveGroupsAsync().SafeFireAndForget();
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
            SaveGroupsAsync().SafeFireAndForget();
        }

        private async Task AddGroupFromFiles(IList<string> files)
        {
            if (files.Count == 0)
                return;

            var group = new AssetGroup(GenericGroupName());

            await AddAssetsToGroup(group, files);
            AssetGroups.Add(group);
            SaveGroupsAsync().SafeFireAndForget();
        }

        private async Task AddAssetsToGroup(AssetGroup group, IEnumerable<string> files)
        {
            var assets = await Task.Run(() => _assetManager.Load(files));
            group.AddAssets(assets, DuplicateHandling.Deny);
            SaveGroupsAsync().SafeFireAndForget();
        }

        private string GenericGroupName()
        {
            string group = Localization.Instance["Group"];
            int genericGroupCount = AssetGroups.Select(g => g.Name.Contains(group, StringComparison.OrdinalIgnoreCase)).Count();
            return genericGroupCount == 0 ? group : group + " " + (genericGroupCount + 1);
        }
    }
}
