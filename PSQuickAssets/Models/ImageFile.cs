using System.Windows.Media.Imaging;

namespace PSQuickAssets.Models
{
    public class ImageFile
    {
        public BitmapSource Thumbnail { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string Name { get; set; }
    }
}
