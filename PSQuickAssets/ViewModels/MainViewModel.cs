using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using PropertyChanged;
using PSQuickAssets.Infrastructure;
using PSQuickAssets.Models;

namespace PSQuickAssets.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class MainViewModel
    {
        public ObservableCollection<List<ImageFile>> Folders { get; set; }

        public bool IsWindowShowing { get; set; }
        public List<string> CurrentDirectories { get; set; }

        public ICommand PlaceImageCommand { get; }
        public ICommand ChangeFolderCommand { get; }
        public ICommand HideCommand { get; }
        public ICommand RemoveFolderCommand { get; }

        private IImagesLoader _imagesLoader;

        public MainViewModel(IImagesLoader imagesLoader)
        {
            _imagesLoader = imagesLoader;

            CurrentDirectories = ConfigManager.GetCurrentDirectories();

            PlaceImageCommand = new RelayCommand(path => PlaceImage((string)path));
            ChangeFolderCommand = new RelayCommand(_ => ChangeDirectory());
            HideCommand = new RelayCommand(_ => IsWindowShowing = false);
            RemoveFolderCommand = new RelayCommand(folder => RemoveFolder((List<ImageFile>)folder));

            Folders = new ObservableCollection<List<ImageFile>>();

            foreach (var path in CurrentDirectories)
            {
                LoadImages(path);
            }
        }

        private void RemoveFolder(List<ImageFile> folder)
        {
            Folders.Remove(folder);

            // Remove folderpath from stored directories
            var dir = Path.GetDirectoryName(folder[0].FilePath);
            CurrentDirectories.Remove(dir);
            UpdateConfig();
        }

        private async void PlaceImage(string filePath)
        {
            IsWindowShowing = false;

            bool isPlaced = await PhotoshopManager.PlaceImageAcync(filePath);

            if (!isPlaced)
                IsWindowShowing = true;
        }

        private void LoadImages(string path)
        {
            Folders.Add(_imagesLoader.LoadImages(path));
        }

        private void ChangeDirectory()
        {
            string newDirectory = ViewManager.ShowSelectDirectoryDialog();

            if (string.IsNullOrWhiteSpace(newDirectory))
                return;

            CurrentDirectories.Add(newDirectory);
            LoadImages(newDirectory);
            UpdateConfig();
        }

        private void UpdateConfig()
        {
            ConfigManager.Config = ConfigManager.Config with { Directories = CurrentDirectories };
            ConfigManager.Write();
        }
    }
}
