using MLogger;
using PSQuickAssets.Assets.Thumbnails;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace PSQuickAssets.Assets;

/// <summary>
/// Provides ability to populate <see cref="Asset"/> with data.
/// </summary>
internal class AssetDataLoader
{
    private readonly ThumbnailManager _thumbnailManager;
    private readonly ILogger _logger;

    /// <summary>
    /// Creates an instance of a loader.
    /// </summary>
    public AssetDataLoader(ThumbnailManager thumbnailManager, ILogger logger)
    {
        _thumbnailManager = thumbnailManager;
        _logger = logger;
    }

    /// <summary>
    /// Loads new data to provided <see cref="Asset"/>.
    /// </summary>
    /// <param name="asset">Object that will be loaded with data.</param>
    /// <returns>Same object with modified properties.</returns>
    public Asset LoadData(Asset asset)
    {
        asset.Thumbnail = _thumbnailManager.Create(asset.Path);
        var (dimensions, size) = GetImageInfo(asset.Path);
        asset.Dimensions = dimensions;
        asset.FileSize = size;
        return asset;
    }

    /// <summary>
    /// Opens the image file and reads necessary info from it.
    /// </summary>
    /// <param name="filePath">Full path to the image file.</param>
    /// <returns>Info that was read from a file. Result will be '-1' if failed.</returns>
    private (Size dimensions, long size) GetImageInfo(string filePath)
    {
        try
        {
            using var imageStream = File.OpenRead(filePath);
            var decoder = BitmapDecoder.Create(imageStream, BitmapCreateOptions.IgnoreColorProfile, BitmapCacheOption.Default);
            Size dim = new Size(decoder.Frames[0].PixelWidth, decoder.Frames[0].PixelHeight);
            long bytes = imageStream.Length;
            return (dim, bytes);
        }
        catch (Exception ex)
        {
            _logger.Error($"[Asset Data Loading] Failed to load image file info: {ex.Message}");
            return (new Size(-1, -1), -1);
        }
    }
}