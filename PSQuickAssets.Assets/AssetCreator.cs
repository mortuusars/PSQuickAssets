using MLogger;
using System.IO;

namespace PSQuickAssets.Assets;

internal interface IAssetCreator
{
    Asset Create(string filePath);
}

internal class AssetCreator : IAssetCreator
{
    private readonly Dictionary<string, Asset> _loadedAssets;

    private readonly AssetDataLoader _assetDataLoader;
    private readonly ILogger _logger;

    public AssetCreator(AssetDataLoader assetDataLoader, ILogger logger)
    {
        _loadedAssets = new Dictionary<string, Asset>();

        _assetDataLoader = assetDataLoader;
        _logger = logger;
    }

    /// <summary>
    /// Creates asset from specified filePath and loads it with data (Thumbnail, Size, etc.)
    /// </summary>
    /// <param name="filePath">Full path to the image file.</param>
    /// <returns>Created asset.</returns>
    /// <exception cref="ArgumentNullException">If 'filePath' is null or empty.</exception>
    /// <exception cref="FileNotFoundException">If file does not exists or cannot be opened.</exception>
    /// <exception cref="Exception">If creating fails.</exception>
    public Asset Create(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentNullException(nameof(filePath));

        if (!File.Exists(filePath))
            throw new FileNotFoundException(filePath);

        if (_loadedAssets.ContainsKey(filePath))
            return _loadedAssets[filePath];

        Asset asset = new Asset(filePath);
        _assetDataLoader.LoadData(asset);

        _loadedAssets.Add(asset.Path, asset);

        return asset;
    }
}