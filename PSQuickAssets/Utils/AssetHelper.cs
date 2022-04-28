using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace PSQuickAssets.Utils;

internal static class AssetHelper
{
    /// <summary>
    /// Gets the image Width and Height in pixels.
    /// </summary>
    /// <param name="filePath">Full path to the image.</param>
    /// <returns>Size of the image or <see cref="Size.Empty"/> if failed.</returns>
    public static Size GetImageDimensions(string filePath)
    {
        try
        {
            using var imageStream = File.OpenRead(filePath);
            var decoder = BitmapDecoder.Create(imageStream, BitmapCreateOptions.IgnoreColorProfile, BitmapCacheOption.Default);
            return new(decoder.Frames[0].PixelWidth, decoder.Frames[0].PixelHeight);
        }
        catch (Exception)
        {
            return Size.Empty;
        }
    }
}
