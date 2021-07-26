using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Timers;
using System.Windows.Input;
using PropertyChanged;
using PSQuickAssets.Infrastructure;
using PSQuickAssets.Models;
using PSQuickAssets.Services;
using PSQuickAssets.Utils;

namespace PSQuickAssets.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class MainViewModel
    {
        public ObservableCollection<List<ImageFile>> Folders { get; set; } = new();
        public List<string> CurrentDirectories { get; set; } = new();

        public string Error { get; set; } = "";
        public bool IsWindowShowing { get; set; }
        private readonly Timer _errorShowingTimer = new Timer();

        public ICommand PlaceImageCommand { get; }
        public ICommand ChangeFolderCommand { get; }
        public ICommand HideCommand { get; }
        public ICommand RemoveFolderCommand { get; }

        private readonly IImagesLoader _imagesLoader;

        public MainViewModel(IImagesLoader imagesLoader)
        {
            _imagesLoader = imagesLoader;

            PlaceImageCommand = new RelayCommand(path => PlaceImage((string)path));
            ChangeFolderCommand = new RelayCommand(_ => AddNewDirectory());
            HideCommand = new RelayCommand(_ => IsWindowShowing = false);
            RemoveFolderCommand = new RelayCommand(folder => RemoveFolder((List<ImageFile>)folder));

            foreach (var path in ConfigManager.GetCurrentDirectories())
            {
                LoadDirectory(path);
            }
        }

        private void RemoveFolder(List<ImageFile> folder)
        {
            Folders.Remove(folder);

            // Remove folderpath from stored directories
            var dir = Path.GetDirectoryName(folder[0].FilePath);
            CurrentDirectories.Remove(dir);
            UpdateConfig();
            Sound.Click();
        }

        private async void PlaceImage(string filePath)
        {
            IsWindowShowing = false;

            PSResult callResult = await new PhotoshopManager().AddImageToDocAsync(filePath);

            if (callResult.CallResult != PSCallResult.Success)
            {
                IsWindowShowing = true;
                SetError(callResult.Message);
                Sound.Error();
            }
            else
            {
                WindowControl.FocusPSWindow();
            }
        }

        private List<ImageFile> LoadImages(string path)
        {
            return _imagesLoader.LoadImages(path);
        }

        private void AddNewDirectory()
        {
            string newDirectoryPath = ViewManager.ShowSelectDirectoryDialog();
            if (string.IsNullOrWhiteSpace(newDirectoryPath))
                return;

            LoadDirectory(newDirectoryPath);
            Sound.Click();
        }

        private void LoadDirectory(string directory)
        {
            List<ImageFile> images = LoadImages(directory);

            if (images.Count > 0)
            {
                Folders.Add(images);
                CurrentDirectories.Add(directory);
                UpdateConfig();
            }
            else
            {
                SetError("No valid images in a folder");
                Sound.Error();
            }
        }

        private void UpdateConfig()
        {
            ConfigManager.Config = ConfigManager.Config with { Directories = CurrentDirectories };
            ConfigManager.Write();
        }

        private void SetError(string errorMessage)
        {
            _errorShowingTimer.Stop();
            _errorShowingTimer.Interval = 3000;
            Error = errorMessage;
            _errorShowingTimer.Elapsed += (s, e) => { Error = ""; _errorShowingTimer.Stop(); };
            _errorShowingTimer.Start();
        }
    }
}
