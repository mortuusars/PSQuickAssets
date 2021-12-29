using MLogger;
using PSQuickAssets.Assets.Thumbnails;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace PSQuickAssets.Assets;

internal interface IAssetLoader
{
    Asset Load(string filePath);
}

internal class AssetLoader : IAssetLoader
{
    private Dictionary<string, Asset> _loadedAssets;

    private readonly IThumbnailCreator _thumbnailCreator;
    private readonly ILogger _logger;

    public AssetLoader(IThumbnailCreator thumbnailCreator, ILogger logger)
    {
        _loadedAssets = new Dictionary<string, Asset>();

        _thumbnailCreator = thumbnailCreator;
        _logger = logger;
    }

    /// <summary>
    /// Creates asset with thumbnail from an image file.
    /// </summary>
    /// <param name="filePath">Full path to the image file.</param>
    /// <returns>Created asset.</returns>
    /// <exception cref="ArgumentNullException">If 'filePath' is null or empty.</exception>
    /// <exception cref="Exception">If creating fails.</exception>
    public Asset Load(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentNullException(nameof(filePath));

        if (_loadedAssets.ContainsKey(filePath))
            return _loadedAssets[filePath];

        Asset asset = CreateAsset(filePath);

        return asset;
    }

    private Asset CreateAsset(string filePath)
    {
        Asset asset = new Asset
        {
            Thumbnail = CreateThumbnail(filePath),
            Path = filePath,
            FileSize = new FileInfo(filePath).Length,
            Dimensions = GetAssetDimensions(filePath)
        };

        _loadedAssets.Add(asset.Path, asset);

        return asset;
    }

    private BitmapImage? CreateThumbnail(string filePath)
    {
        try
        {
            return _thumbnailCreator.CreateThumbnail(filePath, 400, 90);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to create thumbnail for '{filePath}':\n{ex}");
            return null;
        }
    }

    private Size GetAssetDimensions(string filePath)
    {
        try
        {
            using (var imageStream = File.OpenRead(filePath))
            {
                var decoder = BitmapDecoder.Create(imageStream, BitmapCreateOptions.IgnoreColorProfile, BitmapCacheOption.Default);
                return new Size(decoder.Frames[0].PixelWidth, decoder.Frames[0].PixelHeight);
            }
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to get image dimensions for '{filePath}':\n{ex}");
            return new Size(-1, -1);
        }
    }
}