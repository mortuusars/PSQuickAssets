using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
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
            string[] validFormats = new string[] { ".jpg", "jpeg", ".png", "bmp" };
            List<ImageFile> images = new List<ImageFile>();

            foreach (var filePath in GetDirectoryFiles(directoryPath))
            {
                if (Array.Exists(validFormats, type => type == Path.GetExtension(filePath)))
                    images.Add(CreateImageFile(filePath, imageSize, constrainTo));
            }

            return images;
        }

        private static ImageFile CreateImageFile(string filePath, int imageSize, ConstrainTo constrainTo)
        {
            var imageFile =  new ImageFile
            {
                Thumbnail = constrainTo == ConstrainTo.Width ? CreateThumbnailToWidth(filePath, imageSize) : CreateThumbnailToHeight(filePath, imageSize),
                FilePath = filePath,
                FileName = Path.GetFileNameWithoutExtension(filePath)
            };
            
            imageFile.ShortFileName = StringFormatter.CutStart(input: Path.GetFileNameWithoutExtension(filePath), 
                                                               numberOfChars: imageFile.Thumbnail.PixelWidth);

            return imageFile;
        }

        private static int CalculateCharsNumber(int pixelWidth)
        {
            return (int)Math.Max(20, Math.Min(160, (pixelWidth / 3.5) * 1.2));
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

        private static BitmapImage CreateThumbnailToWidth(string filepath, int width = 60)
        {
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new Uri(filepath);
            bmp.DecodePixelWidth = width;
            bmp.EndInit();

            return bmp;
        }

        private static BitmapImage CreateThumbnailToHeight(string filepath, int height = 60)
        {
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new Uri(filepath);
            bmp.DecodePixelHeight = height;
            bmp.EndInit();

            return bmp;
        }
    }
}
