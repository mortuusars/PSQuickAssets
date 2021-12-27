using System.Windows.Media.Imaging;

namespace PSQuickAssets.Assets.Thumbnails;

internal interface IThumbnailCreator
{
    /// <summary>
    /// Creates a BitmapImage from given image file.
    /// </summary>
    /// <param name="filePath">Full path to the image file.</param>
    /// <param name="maxWidth">Thumbnail size will be constrained to this width.</param>
    /// <param name="maxHeight">Thumbnail size will be constrained to this height.</param>
    /// <exception cref="Exception">Can throw exceptions.</exception>
    BitmapImage CreateThumbnail(string filePath, int maxWidth, int maxHeight);
}