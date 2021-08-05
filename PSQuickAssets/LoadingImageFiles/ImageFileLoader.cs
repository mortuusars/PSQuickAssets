using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using PSQuickAssets.LoadingImageFiles;
using PSQuickAssets.Models;

namespace PSQuickAssets
{
    public enum ConstrainTo
    {
        Width, Height
    }

    public class ImageFileLoader : IImageFileLoader
    {
        public async Task<List<ImageFile>> LoadAsync(string directoryPath, int imageSize, ConstrainTo constrainTo)
        {
            return await Task.Run(() => Load(directoryPath, imageSize, constrainTo));
        }

        public List<ImageFile> Load(string directoryPath, int imageSize, ConstrainTo constrainTo)
        {
            string[] validFormats = new string[] { ".jpg", ".jpeg", ".png", ".bmp", ".tiff", ".tif"};
            List<ImageFile> images = new List<ImageFile>();

            var timeBefore = DateTime.Now;

            foreach (var filePath in GetDirectoryFiles(directoryPath))
            {
                string extension = Path.GetExtension(filePath);

                if (Array.Exists(validFormats, type => type == extension))
                    images.Add(CreateImageFile(filePath, imageSize, constrainTo));
                else if (extension == ".psd" || extension == ".psb")
                    images.Add(CreatePhotoshopImageFile(filePath, extension, imageSize, constrainTo));
            }

            var afterTime = DateTime.Now;
            Debug.WriteLine($"Loading {directoryPath} took: {(afterTime - timeBefore).TotalSeconds} seconds.");

            return images;
        }

        private static ImageFile CreatePhotoshopImageFile(string filePath, string format, int imageSize, ConstrainTo constrainTo)
        {
            return new ImageFile()
            {
                Thumbnail = ThumbnailCreator.FromResource(
                    new Uri($"pack://application:,,,/Resources/Images/{format.Replace(".", "")}_90.png"), imageSize, constrainTo),
                FilePath = filePath,
                FileName = Path.GetFileName(filePath)
            };
        }

        private static ImageFile CreateImageFile(string filePath, int imageSize, ConstrainTo constrainTo)
        {
            return new ImageFile
            {
                Thumbnail = ThumbnailCreator.FromFile(filePath, imageSize, constrainTo),
                FilePath = filePath,
                FileName = Path.GetFileName(filePath)
            };
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
    }
}
