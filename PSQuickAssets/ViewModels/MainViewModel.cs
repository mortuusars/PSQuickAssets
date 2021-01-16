using System;
using System.Collections.Generic;
using System.Windows.Input;
using PropertyChanged;
using PSQuickAssets.Infrastructure;
using PSQuickAssets.Models;

namespace PSQuickAssets.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class MainViewModel
    {
        public List<ImageFileInfo> FilesList { get; private set; }

        public int FilesCount { get; set; }
        public bool IsWindowShowing { get; set; }
        public string CurrentDirectoryPath { get; set; }

        public ICommand PlaceImageCommand { get; }
        public ICommand ChangeFolderCommand { get; }
        public ICommand HideCommand { get; }

        private IFileImageFileManager _fileRecordManager;

        public MainViewModel(IFileImageFileManager fileRecordManager)
        {
            _fileRecordManager = fileRecordManager;

            CurrentDirectoryPath = ConfigManager.GetFilesDirectory();

            PlaceImageCommand = new RelayCommand(path => OnPlaceImageCommand((string)path));
            ChangeFolderCommand = new RelayCommand(_ => ChangeDirectory());
            HideCommand = new RelayCommand(_ => IsWindowShowing = false);

            GetImagesAsync();
        }

        private async void OnPlaceImageCommand(string filePath)
        {
            IsWindowShowing = false;

            bool isPlaced = await PhotoshopManager.PlaceImageAcync(filePath);

            if (!isPlaced)
                IsWindowShowing = true;
        }

        private async void GetImagesAsync()
        {
            FilesList = await _fileRecordManager.GetFilesAsync(CurrentDirectoryPath);
        }

        private void ChangeDirectory()
        {
            string newDirectory = ViewManager.ShowSelectDirectoryDialog();

            if (string.IsNullOrWhiteSpace(newDirectory))
                return;

            CurrentDirectoryPath = newDirectory;
            GetImagesAsync();

            ConfigManager.Config = ConfigManager.Config with { Directory = CurrentDirectoryPath };
            ConfigManager.Write();
        }
    }
}
