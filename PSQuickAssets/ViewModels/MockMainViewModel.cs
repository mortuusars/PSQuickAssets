using System.Collections.Generic;
using PropertyChanged;
using PSQuickAssets.Models;

namespace PSQuickAssets.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class MockMainViewModel
    {
        public List<ImageFileInfo> FilesList { get; private set; } = new List<ImageFileInfo>() 
        {
            new ImageFileInfo() { FileName = "Fire1", FilePath = "F:\\Work\\Replacement\\Fire1.jpg", ShortFileName = "Fire1"} 
        };

        public int FilesCount { get; set; }
        public bool IsWindowShowing { get; set; }
        //public string CurrentDirectoryPath { get; set; }

        public MockMainViewModel()
        {
            //CurrentDirectoryPath = ConfigManager.GetFilesDirectory();

            GetImagesAsync();
        }

        private async void GetImagesAsync()
        {
            //FilesList = await _fileRecordManager.GetFilesAsync(CurrentDirectoryPath);
        }
    }
}
