using GongSolutions.Wpf.DragDrop;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using PSQuickAssets.Assets;
using PSQuickAssets.Configuration;
using PSQuickAssets.Models;
using PSQuickAssets.PSInterop;
using PSQuickAssets.Services;
using PSQuickAssets.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Media;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;

namespace PSQuickAssets.ViewModels
{
    public class MainViewModel : ObservableObject, IDropTarget
    {
        public bool AlwaysOnTop { get => _config.AlwaysOnTop; }

        public AssetsViewModel AssetsViewModel { get; }

        public ObservableCollection<List<ImageFile>> Folders { get; set; } = new();
        public List<string> CurrentDirectories { get; set; } = new();

        public double ThumbnailSize { get; } = 55;

        public ICommand SettingsCommand { get; }
        public ICommand HideCommand { get; }
        public ICommand ShutdownCommand { get; } = new RelayCommand(_ => App.Current.Shutdown());

        private readonly IImageFileLoader _imagesLoader;
        private readonly WindowManager _windowManager;
        private readonly INotificationService _notificationService;
        private readonly Config _config;

        internal MainViewModel(IImageFileLoader imagesLoader, WindowManager windowManager, INotificationService notificationService, Config config)
        {
            AssetsViewModel = new AssetsViewModel(new AssetLoader(), new AssetAtlas(new AtlasLoader(), new AtlasSaver(), "save.json"), windowManager, notificationService, config);

            _imagesLoader = imagesLoader;
            _windowManager = windowManager;
            _notificationService = notificationService;
            _config = config;

            _config.ConfigPropertyChanged += (property) => { if (property.Equals(nameof(_config.AlwaysOnTop))) OnPropertyChanged(nameof(AlwaysOnTop)); };

            SettingsCommand = new RelayCommand(_ => _windowManager.ShowSettingsWindow());
            HideCommand = new RelayCommand(_ => _windowManager.HideMainWindow());
        }

        private async Task<List<ImageFile>> LoadImages(string path)
        {
            return await _imagesLoader.LoadAsync(path, (int)ThumbnailSize, ConstrainTo.Height);
        }

        private async Task<bool> LoadDirectory(string directory)
        {
            List<ImageFile> images = await LoadImages(directory);

            if (images.Count > 0)
            {
                Folders.Add(images);
                CurrentDirectories.Add(directory);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void DragOver(IDropInfo dropInfo)
        {
            DataObject dataObject = dropInfo.Data as DataObject;
            string[] paths = dataObject.GetData(DataFormats.FileDrop) as string[];

            if (paths != null)
            {
                dropInfo.Effects = DragDropEffects.Move;
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
            }
            else
                dropInfo.Effects = DragDropEffects.None;
        }

        public void Drop(IDropInfo dropInfo)
        {
            DataObject dataObject = dropInfo.Data as DataObject;
            string[] paths = dataObject.GetData(DataFormats.FileDrop) as string[];

            LoadDroppedDirectories(paths);
        }

        private void LoadDroppedDirectories(string[] paths)
        {
            if (paths == null || paths.Length == 0)
                return;

            List<string> files = new();
            List<string> folders = new();

            foreach (string path in paths)
            {
                if (Path.HasExtension(path))
                    files.Add(path);
                else
                    folders.Add(path);
            }

            //AddAssetsFromFiles(files);
            AddAssetsFromFolders(folders);
        }

        private async void AddAssetsFromFolders(IEnumerable<string> folders)
        {
            foreach (string folder in folders)
            {
                if (!CurrentDirectories.Contains(folder))
                    await LoadDirectory(folder);

                string[] nestedDirs = Directory.GetDirectories(folder);
                LoadDroppedDirectories(nestedDirs);
            }
        }
    }
}
