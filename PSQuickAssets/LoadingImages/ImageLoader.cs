using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using PSQuickAssets.Models;

namespace PSQuickAssets
{
    public class ImagesLoader : IImagesLoader
    {
        public async Task<List<ImageFile>> LoadImagesAsync(string directoryPath)
        {
            return await Task.Run(() => LoadImages(directoryPath));
        }

        public List<ImageFile> LoadImages(string directoryPath)
        {
            string[] validFormats = new string[] { ".jpg", "jpeg", ".png", "bmp" };
            List<ImageFile> images = new List<ImageFile>();

            foreach (var filePath in GetDirectoryFiles(directoryPath))
            {
                if (Array.Exists(validFormats, type => type == Path.GetExtension(filePath)))
                    images.Add(CreateImageFile(filePath));
            }

            return images;
        }

        private ImageFile CreateImageFile(string filePath)
        {
            ImageFile img = new ImageFile();
            img.Thumbnail = LoadThumbnail(filePath, 50);
            img.FilePath = filePath;
            img.FileName = Path.GetFileNameWithoutExtension(filePath);
            img.ShortFileName = StringFormatter.CutStart(img.FileName, 20);
            return img;
        }

        private string[] GetDirectoryFiles(string directoryPath)
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

        private static BitmapImage LoadThumbnail(string filepath, int width = 45)
        {
            Image img = new Image();
            img.Width = 200;

            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new Uri(filepath);
            bmp.DecodePixelWidth = width;
            bmp.EndInit();

            return bmp;
        }
    }
}
