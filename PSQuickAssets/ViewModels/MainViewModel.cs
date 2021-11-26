using GongSolutions.Wpf.DragDrop;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using PSQuickAssets.Assets;
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
        public AssetsViewModel AssetsViewModel { get; }

        public ObservableCollection<List<ImageFile>> Folders { get; set; } = new();
        public List<string> CurrentDirectories { get; set; } = new();

        public double ThumbnailSize { get; } = 55;

        public ICommand PlaceImageCommand { get; }
        public ICommand PlaceImageWithMaskCommand { get; }
        public ICommand SettingsCommand { get; }
        public ICommand AddFolderCommand { get; }
        public ICommand RemoveFolderCommand { get; }
        public ICommand HideCommand { get; }
        public ICommand ShutdownCommand { get; } = new RelayCommand(_ => App.Current.Shutdown());

        private readonly IImageFileLoader _imagesLoader;
        private readonly WindowManager _viewManager;
        private readonly INotificationService _notificationService;

        internal MainViewModel(IImageFileLoader imagesLoader, WindowManager viewManager, INotificationService notificationService)
        {
            AssetsViewModel = new AssetsViewModel(new AssetLoader(), new AssetAtlas(new AtlasLoader(), new AtlasSaver(), "save.json"), viewManager);

            _imagesLoader = imagesLoader;
            _viewManager = viewManager;
            _notificationService = notificationService;
            PlaceImageCommand = new RelayCommand(path => PlaceImage((string)path));
            PlaceImageWithMaskCommand = new RelayCommand(path => PlaceImageWithMaskAsync((string)path));

            SettingsCommand = new RelayCommand(_ => _viewManager.ShowSettingsWindow());
            AddFolderCommand = new RelayCommand(_ => AddNewDirectoryAsync());
            RemoveFolderCommand = new RelayCommand(folder => RemoveFolder((List<ImageFile>)folder));
            HideCommand = new RelayCommand(_ => _viewManager.HideMainWindow());

            LoadSavedDirs();
        }

        private async void PlaceImage(string filePath)
        {
            _viewManager.ToggleMainWindow();

            IPhotoshopInterop photoshopInterop = new PhotoshopInterop();
            WindowControl.FocusWindow("photoshop");

            PSResult psResult = await Task.Run(() => photoshopInterop.AddImageToDocument(filePath));

            if (psResult.Status == PSStatus.NoDocumentsOpen)
                psResult = await Task.Run(() => photoshopInterop.OpenImage(filePath));

            if (psResult.Status != PSStatus.Success)
            {
                _viewManager.ToggleMainWindow();
                _notificationService.Notify(App.AppName, psResult.ResultMessage, NotificationIcon.Error);
            }
        }

        private async void PlaceImageWithMaskAsync(string filePath)
        {
            _viewManager.ToggleMainWindow();

            IPhotoshopInterop photoshopInterop = new PhotoshopInterop();
            WindowControl.FocusWindow("photoshop");

            PSResult psResult = await Task.Run(() => photoshopInterop.AddImageToDocumentWithMask(filePath, MaskMode.RevealSelection));

            if (psResult.Status == PSStatus.NoSelection)
                psResult = await Task.Run(() => photoshopInterop.AddImageToDocument(filePath));

            if (psResult.Status == PSStatus.NoDocumentsOpen)
            {
                psResult = await Task.Run(() => photoshopInterop.OpenImage(filePath));
                return;
            }

            if (psResult.Status != PSStatus.Success)
            {
                _viewManager.ToggleMainWindow();
                _notificationService.Notify(App.AppName, psResult.ResultMessage, NotificationIcon.Error);
                return;
            }

            await ExecuteActionAsync(photoshopInterop, "SelectRGBLayer", "Mask");
            //await ExecuteActionAsync(photoshopInterop, "FreeTransform", "General");
        }

        private async Task ExecuteActionAsync(IPhotoshopInterop photoshopInterop, string action, string from)
        {
            PSResult psResult = await Task.Run(() => photoshopInterop.ExecuteAction(action, from));

            if (psResult.Status != PSStatus.Success)
            {
                _viewManager.ToggleMainWindow();
                string errorMessage = String.Format(Resources.Localization.Instance["Assets_CannotExecuteActionFromSet"], action, from) + $"\n{psResult.ResultMessage}";
                _notificationService.Notify(App.AppName, errorMessage, NotificationIcon.Error);
            }
        }

        private async void LoadSavedDirs()
        {
            //foreach (var path in ConfigManager.Config.Directories)
                //await LoadDirectory(path);
        }

        private async Task<List<ImageFile>> LoadImages(string path)
        {
            return await _imagesLoader.LoadAsync(path, (int)ThumbnailSize, ConstrainTo.Height);
        }

        private void AddNewDirectoryAsync()
        {
            ////string newDirectoryPath = _viewManager.ShowSelectDirectoryDialog();
            //if (string.IsNullOrWhiteSpace(newDirectoryPath))
            //    return;

            //if (CurrentDirectories.Contains(newDirectoryPath))
            //{
            //    ShowError($"\"{new DirectoryInfo(newDirectoryPath).Name}\" is already added");
            //    return;
            //}

            //if (await LoadDirectory(newDirectoryPath))
            //    Sound.Click();
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

        private void RemoveFolder(List<ImageFile> folder)
        {
            Folders.Remove(folder);

            // Remove folderpath from stored directories
            var dir = Path.GetDirectoryName(folder[0].FilePath);
            CurrentDirectories.Remove(dir);
            Sound.Click();
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
