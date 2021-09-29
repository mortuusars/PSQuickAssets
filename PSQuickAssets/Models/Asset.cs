using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace PSQuickAssets.Models
{
    public class Asset
    {
        public BitmapImage Thumbnail { get; set; }
        public string ThumbnailPath { get; set; }
        public string FilePath { get; set; }
        public string FileName { get => Path.GetFileName(FilePath); }
        public string Name { get => Path.GetFileNameWithoutExtension(FilePath); }
        public string Format { get => Path.GetExtension(FilePath); }
        public Size Dimensions { get; set; }
        public int Width { get => Dimensions.Width; }
        public int Height { get => Dimensions.Height; }
        public long Size { get; set; }
    }
}
