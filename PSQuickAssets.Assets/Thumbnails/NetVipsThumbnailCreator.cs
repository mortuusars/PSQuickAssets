using System.IO;
using System.Windows.Media.Imaging;

namespace PSQuickAssets.Assets.Thumbnails
{
    internal class NetVipsThumbnailCreator : IThumbnailCreator
    {
        public BitmapImage CreateThumbnail(string filePath, int maxWidth, int maxHeight)
        {
            byte[] imgBytes = GetImageBytesInMemory(filePath, maxWidth, maxHeight);
            BitmapImage bitmapImage = BitmapImageFromBytes(imgBytes);

            //if (bitmapImage.DpiX != 96)
            //    bitmapImage = ConvertBitmapTo96DPI(bitmapImage);

            return bitmapImage;
        }

        private byte[] GetImageBytesInMemory(string filePath, int maxWidth, int maxHeight)
        {
            var image = NetVips.Image.Thumbnail(filePath, maxWidth, maxHeight);
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

        private static BitmapImage ConvertBitmapTo96DPI(BitmapImage bitmapImage)
        {
            double dpi = 96;
            int width = bitmapImage.PixelWidth;
            int height = bitmapImage.PixelHeight;

            int stride = width * bitmapImage.Format.BitsPerPixel;
            byte[] pixelData = new byte[stride * height];
            bitmapImage.CopyPixels(pixelData, stride, 0);

            BitmapSource bitmapSource = BitmapSource.Create(width, height, dpi, dpi, bitmapImage.Format, null, pixelData, stride);

            PngBitmapEncoder encoder = new();
            MemoryStream memoryStream = new();
            BitmapImage image = new ();

            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
            encoder.Save(memoryStream);

            using (var ms = new MemoryStream())
            {
                image.BeginInit();
                image.StreamSource = memoryStream;
                image.EndInit();
                image.Freeze();
            }

            return image;
        }
    }
}
