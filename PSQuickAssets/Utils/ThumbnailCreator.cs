using System;
using System.Windows.Media.Imaging;

namespace PSQuickAssets
{
    public enum ConstrainTo
    {
        Width,
        Height
    }

    /// <summary>
    /// Provides funcionality to create thumbnails from filepath or resourcepath.
    /// </summary>
    public static class ThumbnailCreator
    {
        //TODO: Cleanup
        public static BitmapImage FromFile(string filepath, int maxSize, ConstrainTo constrainTo)
        {
            return constrainTo switch
            {
                ConstrainTo.Width => ToWidth(filepath, maxSize),
                ConstrainTo.Height => ToHeight(filepath, maxSize),
                _ => throw new ArgumentException("Not a valid constrain.")
            };
        }

        public static BitmapImage FromResource(Uri resourceUri, int maxSize, ConstrainTo constrainTo)
        {
            return constrainTo switch
            {
                ConstrainTo.Width => ToWidthFromResource(resourceUri, maxSize),
                ConstrainTo.Height => ToHeightFromResource(resourceUri, maxSize),
                _ => throw new ArgumentException("Not a valid constrain.")
            };
        }

        private static BitmapImage ToWidth(string filepath, int width = 60)
        {
            BitmapImage bmp = new();
            bmp.BeginInit();
            bmp.UriSource = new Uri(filepath);
            bmp.DecodePixelWidth = width;
            bmp.EndInit();
            bmp.Freeze();
            return bmp;
        }

        private static BitmapImage ToHeight(string filepath, int height = 60)
        {
            BitmapImage bmp = new();
            bmp.BeginInit();
            bmp.DecodePixelHeight = height;
            bmp.UriSource = new Uri(filepath);
            bmp.CacheOption = BitmapCacheOption.OnDemand;
            bmp.EndInit();
            bmp.Freeze();
            return bmp;
        }

        private static BitmapImage ToWidthFromResource(Uri uri, int width = 60)
        {
            BitmapImage bmp = new();
            bmp.BeginInit();
            bmp.UriSource = uri;
            bmp.DecodePixelWidth = width;
            bmp.EndInit();
            bmp.Freeze();
            return bmp;
        }

        private static BitmapImage ToHeightFromResource(Uri uri, int height = 60)
        {
            BitmapImage bmp = new();
            bmp.BeginInit();
            bmp.UriSource = uri;
            bmp.DecodePixelHeight = height;
            bmp.EndInit();
            bmp.Freeze();
            return bmp;
        }
    }
}
