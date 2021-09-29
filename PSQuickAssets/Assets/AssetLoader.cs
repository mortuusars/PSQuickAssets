using PSQuickAssets.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PSQuickAssets.Assets
{
    public class AssetLoader
    {
        public Asset Load(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException(nameof(filePath));

            string extension = Path.GetExtension(filePath);

            return IsFormatSupported(Path.GetExtension(filePath))
                ? CreateAsset(filePath)
                : throw new NotSupportedException($"Loading {extension} files is not supported.");
        }

        public bool TryLoad(string filePath, out Asset asset)
        {
            try
            {
                asset = CreateAsset(filePath);
                return true;
            }
            catch (Exception)
            {
                asset = null;
                return false;
            }
        }

        public IEnumerable<Asset> Load(IEnumerable<string> filePaths)
        {
            List<Asset> assets = new();

            foreach (string filePath in filePaths)
            {
                if (TryLoad(filePath, out  Asset asset))
                    assets.Add(asset);
            }

            return assets;
        }

        public bool IsFormatSupported(string fileExtension)
        {
            return fileExtension is ".jpg" or ".jpeg" or ".png" or ".bmp" or ".tiff" or ".tif" or ".psd" or ".psb";
        }

        private Asset CreateAsset(string filePath)
        {
            return new Asset
            {
                Thumbnail = CreateThumbnail(filePath),
                ThumbnailPath = filePath,
                FilePath = filePath,
                Size = new FileInfo(filePath).Length,
                Dimensions = GetAssetPixelSize(filePath)
            };
        }

        private BitmapImage CreateThumbnail(string filePath)
        {
            byte[] imgBytes = GetImageBytesInMemory(filePath, 400, 90);
            return BitmapImageFromBytes(imgBytes);
        }

        private byte[] GetImageBytesInMemory(string filePath, int width, int height)
        {
            var image = NetVips.Image.Thumbnail(filePath, width, height);
            byte[] imgBytes = image.WriteToBuffer(".png");
            image.Dispose();
            return imgBytes;
        }

        private BitmapImage BitmapImageFromBytes(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                BitmapImage bmp = new();
                bmp.BeginInit();
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.StreamSource = ms;
                bmp.EndInit();
                bmp.Freeze();
                return bmp;
            }
        }

        private Size GetAssetPixelSize(string filePath)
        {
            try
            {
                using (var imageStream = File.OpenRead(filePath))
                {
                    var decoder = BitmapDecoder.Create(imageStream, BitmapCreateOptions.IgnoreColorProfile, BitmapCacheOption.Default);
                    return new Size(decoder.Frames[0].PixelWidth, decoder.Frames[0].PixelHeight);
                }
            }
            catch (Exception)
            {
                return new Size(-1, -1);
            }
        }
    }
}
