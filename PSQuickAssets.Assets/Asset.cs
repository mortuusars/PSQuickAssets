using System.IO;

namespace PSQuickAssets.Assets
{
    public class Asset
    {
        public string ThumbnailPath { get; set; }
        public string FilePath { get; set; }
        public string FileName { get => Path.GetFileName(FilePath); }
        public string Name { get => Path.GetFileNameWithoutExtension(FilePath); }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
