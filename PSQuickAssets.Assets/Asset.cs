using System.Drawing;
using System.IO;
using System.Text.Json.Serialization;
using System.Windows.Media.Imaging;

namespace PSQuickAssets.Assets
{
    public class Asset
    {
        /// <summary>Full path to the file.</summary>
        public string Path { get; set; }

        [JsonIgnore]
        public BitmapImage? Thumbnail { get; set; }
        /// <summary>Size of the image in bytes.</summary>
        [JsonIgnore]
        public long FileSize { get; set; }
        [JsonIgnore]
        public Size Dimensions { get; set; }

        [JsonIgnore]
        public int Width { get => Dimensions.Width; }
        [JsonIgnore]
        public int Height { get => Dimensions.Height; }

        [JsonIgnore]
        public string FileName { get => System.IO.Path.GetFileName(Path ?? ""); }
        [JsonIgnore]
        public string Name { get => System.IO.Path.GetFileNameWithoutExtension(Path ?? ""); }
        [JsonIgnore]
        public string Format { get => System.IO.Path.GetExtension(Path ?? ""); }

        public static readonly Asset Empty = new Asset();

        public Asset(string filePath)
        {
            Path = filePath;
        }

        public Asset() : this(string.Empty) { }
    }
}
