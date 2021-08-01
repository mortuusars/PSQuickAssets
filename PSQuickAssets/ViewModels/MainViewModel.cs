using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Media;
using System.Windows.Input;
using PropertyChanged;
using PSQuickAssets.Infrastructure;
using PSQuickAssets.Models;
using PSQuickAssets.Services;
using PSQuickAssets.Utils;
using System.Timers;
using System.Threading.Tasks;

namespace PSQuickAssets.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class MainViewModel
    {
        public ObservableCollection<List<ImageFile>> Folders { get; set; } = new();
        public List<string> CurrentDirectories { get; set; } = new();

        public string Error { get; set; } = "";
        public bool IsWindowShowing { get; set; }
        public double ThumbnailSize { get; } = 55;

        private readonly Timer _errorShowingTimer = new Timer();

        public ICommand PlaceImageCommand { get; }
        public ICommand AddFolderCommand { get; }
        public ICommand RemoveFolderCommand { get; }
        public ICommand HideCommand { get; }
        public ICommand ShutdownCommand { get; } = new RelayCommand(_ => App.Current.Shutdown());

        private readonly IImageFileLoader _imagesLoader;
        private readonly IPhotoshopManager _photoshopManager;

        public MainViewModel(IImageFileLoader imagesLoader, IPhotoshopManager photoshopManager)
        {
            _imagesLoader = imagesLoader;
            _photoshopManager = photoshopManager;

            PlaceImageCommand = new RelayCommand(path => PlaceImage((string)path));
            AddFolderCommand = new RelayCommand(_ => AddNewDirectoryAsync());
            RemoveFolderCommand = new RelayCommand(folder => RemoveFolder((List<ImageFile>)folder));
            HideCommand = new RelayCommand(_ => IsWindowShowing = false);

            LoadSavedDirs();
        }

        private async void LoadSavedDirs()
        {
            foreach (var path in ConfigManager.Config.Directories)
                await LoadDirectory(path);
        }
        
        private async void PlaceImage(string filePath)
        {
            IsWindowShowing = false;

            PSResult psResult = await _photoshopManager.AddImageToDocAsync(filePath);

            if (psResult.CallResult != PSCallResult.Success)
            {
                IsWindowShowing = true;
                ShowError(psResult.Message);
            }
            else
                WindowControl.FocusWindow("photoshop");
        }

        private async Task<List<ImageFile>> LoadImages(string path)
        {
            return await _imagesLoader.LoadAsync(path, (int)ThumbnailSize, ConstrainTo.Height);
        }

        private async void AddNewDirectoryAsync()
        {
            string newDirectoryPath = ViewManager.ShowSelectDirectoryDialog();
            if (string.IsNullOrWhiteSpace(newDirectoryPath))
                return;

            if (CurrentDirectories.Contains(newDirectoryPath))
            {
                ShowError($"\"{new DirectoryInfo(newDirectoryPath).Name}\" is already added");
                return;
            }

            if (await LoadDirectory(newDirectoryPath))
                Sound.Click();
        }

        private async Task<bool> LoadDirectory(string directory)
        {
            List<ImageFile> images = await LoadImages(directory);

            if (images.Count > 0)
            {
                Folders.Add(images);
                CurrentDirectories.Add(directory);
                UpdateConfig();
                return true;
            }
            else
            {
                ShowError("No valid images in a folder");
                return false;
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

        private void UpdateConfig()
        {
            ConfigManager.Config = ConfigManager.Config with { Directories = CurrentDirectories };
            ConfigManager.Save();
        }

        private void ShowError(string errorMessage)
        {
            _errorShowingTimer.Stop();
            _errorShowingTimer.Interval = 3000;
            Error = errorMessage;
            _errorShowingTimer.Elapsed += (s, e) => { Error = ""; _errorShowingTimer.Stop(); };
            _errorShowingTimer.Start();
            SystemSounds.Asterisk.Play();
        }
    }
}
