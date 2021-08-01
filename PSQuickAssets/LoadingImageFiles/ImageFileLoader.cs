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
            string[] validFormats = new string[] { ".jpg", ".jpeg", ".png", ".bmp", ".tiff", ".tif"};
            List<ImageFile> images = new List<ImageFile>();


            foreach (var filePath in GetDirectoryFiles(directoryPath))
            {
                string extension = Path.GetExtension(filePath);

                if (Array.Exists(validFormats, type => type == extension))
                    images.Add(CreateImageFile(filePath, imageSize, constrainTo));
                else if (extension == ".psd")
                    images.Add(CreatePsdFile(filePath, imageSize, constrainTo));
                else if (extension == ".psb")
                    images.Add(CreatePsbFile(filePath, imageSize, constrainTo));
            }

            return images;
        }

        private static ImageFile CreatePsdFile(string filePath, int imageSize, ConstrainTo constrainTo)
        {
            return new ImageFile()
            {
                Thumbnail = constrainTo == ConstrainTo.Width ? CreateThumnailToWidthFromResource("pack://application:,,,/Resources/Images/psd_90.png", imageSize) :
                                                CreateThumnailToHeightFromResource("pack://application:,,,/Resources/Images/psd_90.png", imageSize),
                FilePath = filePath,
                FileName = Path.GetFileName(filePath)
            };
        }

        private static ImageFile CreatePsbFile(string filePath, int imageSize, ConstrainTo constrainTo)
        {
            return new ImageFile()
            {
                Thumbnail = constrainTo == ConstrainTo.Width ? CreateThumnailToWidthFromResource("pack://application:,,,/Resources/Images/psb_90.png", imageSize) :
                                                CreateThumnailToHeightFromResource("pack://application:,,,/Resources/Images/psb_90.png", imageSize),
                FilePath = filePath,
                FileName = Path.GetFileName(filePath)
            };
        }

        private static ImageFile CreateImageFile(string filePath, int imageSize, ConstrainTo constrainTo)
        {
            return new ImageFile
            {
                Thumbnail = constrainTo == ConstrainTo.Width ? CreateThumbnailToWidth(filePath, imageSize) : CreateThumbnailToHeight(filePath, imageSize),
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

        private static BitmapImage CreateThumbnailToWidth(string filepath, int width = 60)
        {
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new Uri(filepath);
            bmp.DecodePixelWidth = width;
            bmp.EndInit();
            bmp.Freeze();

            return bmp;
        }

        private static BitmapImage CreateThumbnailToHeight(string filepath, int height = 60)
        {
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new Uri(filepath);
            bmp.DecodePixelHeight = height;
            bmp.EndInit();
            bmp.Freeze();

            return bmp;
        }

        private static BitmapImage CreateThumnailToWidthFromResource(string uri, int width = 60)
        {
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new Uri(uri);
            bmp.DecodePixelWidth = width;
            bmp.EndInit();
            bmp.Freeze();
            return bmp;
        }

        private static BitmapImage CreateThumnailToHeightFromResource(string uri, int height = 60)
        {
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new Uri(uri);
            bmp.DecodePixelHeight = height;
            bmp.EndInit();
            bmp.Freeze();
            return bmp;
        }
    }
}
