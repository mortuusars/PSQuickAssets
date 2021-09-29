using PSQuickAssets.Assets;
using PSQuickAssets.Models;
using PSQuickAssets.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace PSQuickAssets.ViewModels
{
    public class AssetsViewModel
    {
        public ObservableCollection<AssetGroup> AssetGroups { get; }

        public ICommand AddFolderCommand { get; }
        public ICommand AddFolderWithSubfoldersCommand { get; }
        public ICommand AddFilesCommand { get; }
        public ICommand RemoveGroupCommand { get; }

        private readonly ViewManager _viewManager;
        private readonly AssetLoader _assetLoader;

        public AssetsViewModel(AssetLoader assetLoader, ViewManager viewManager)
        {
            AssetGroups = new ObservableCollection<AssetGroup>();

            _assetLoader = assetLoader;
            _viewManager = viewManager;

            AddFolderCommand = new RelayCommand(_ => SelectAndAddFolder(includeSubfolders: false));
            AddFolderWithSubfoldersCommand = new RelayCommand(_ => SelectAndAddFolder(includeSubfolders: true));
            AddFilesCommand = new RelayCommand(_ => SelectAndAddFiles());
            RemoveGroupCommand = new RelayCommand(group => RemoveGroup((AssetGroup)group));

            AssetGroups.CollectionChanged += (_, _) => SaveAssets();
        }

        public void SaveAssets()
        {

        }

        public bool AddEmptyGroup(string groupName)
        {
            if (IsGroupExists(groupName))
                return false;

            AssetGroups.Add(new AssetGroup(groupName));
            return true;
        }

        public bool RemoveGroup(AssetGroup assetGroup)
        {
            return AssetGroups.Remove(assetGroup);
        }

        private bool IsGroupExists(string groupName)
        {
            return AssetGroups.Any(group => group.Name == groupName);
        }

        private void SelectAndAddFolder(bool includeSubfolders = false)
        {
            string folderPath = _viewManager.ShowSelectFolderDialog();
            AddGroupFromFolder(folderPath, includeSubfolders);
        }

        private void SelectAndAddFiles()
        {
            string[] files = _viewManager.ShowSelectFilesDialog("Select Assets",
                    "Image Files(*.BMP; *.JPG; *.JPEG; *.GIF; *.TIFF; *.TIF; *.PSD; *.PSB)| " +
                    "*.BMP; *.JPG; *.JPEG; *.GIF; *.TIFF; *.TIF; *.PSD; *.PSB | All files(*.*) | *.*", selectMultiple: true);
            AddGroupFromFiles(files);
        }

        private void AddGroupFromFolder(string folderPath, bool includeSubfolders)
        {
            if (string.IsNullOrEmpty(folderPath))
                return;

            string[] files = GetDirectoryFiles(folderPath);

            AddGroupFromFiles(files);

            if (includeSubfolders)
            {
                string[] folders = Directory.GetDirectories(folderPath);
                foreach (var folder in folders)
                    AddGroupFromFolder(folder, includeSubfolders);
            }
        }

        private void AddGroupFromFiles(IList<string> files)
        {
            if (files.Count == 0)
                return;

            var group = new AssetGroup(GenerateGenericGroupName());

            foreach (var file in files)
            {
                var asset = _assetLoader.Load(file);
                group.AddAsset(asset);
            }

            AssetGroups.Add(group);
        }

        private static string[] GetDirectoryFiles(string directoryPath)
        {
            try
            {
                return Directory.GetFiles(directoryPath);
            }
            catch (Exception)
            {
                return Array.Empty<string>();
            }
        }

        private string GenerateGenericGroupName()
        {
            int genericCount = AssetGroups.Select(g => g.Name.Contains("group", StringComparison.OrdinalIgnoreCase)).Count();

            if (genericCount == 0)
                return "Group";

            return "Group " + (genericCount + 1);
        }
    }
}
