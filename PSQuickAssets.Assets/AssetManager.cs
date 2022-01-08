using MLogger;
using PSQuickAssets.Assets.Thumbnails;
using System.IO;

namespace PSQuickAssets.Assets;

/// <summary>
/// Contains core functionality regarding assets. Creating, saving, loading stored state - it is all here.
/// </summary>
public class AssetManager
{
    private string _savedAssetsFolder;

    private readonly IAssetCreator _assetCreator;
    private readonly IAssetSaver _assetGroupSaver;
    private readonly AssetGroupLoader _assetGroupLoader;
    private readonly ThumbnailManager _thumbnailManager;
    private readonly AssetDataLoader _assetDataLoader;

    private ILogger _logger;

    /// <summary>
    /// Creates an instance of Asset Manager.
    /// </summary>
    /// <param name="savedAssetsFolderPath">Full path to the folder, in which state will be saved and loaded from.</param>
    /// <param name="logger">Logger.</param>
    public AssetManager(string savedAssetsFolderPath, ILogger logger)
    {
        _savedAssetsFolder = savedAssetsFolderPath;
        _logger = logger;

        _thumbnailManager = new ThumbnailManager(logger);
        _assetDataLoader = new AssetDataLoader(_thumbnailManager, logger);

        _assetCreator = new AssetCreator(_assetDataLoader, logger);
        _assetGroupSaver = new AssetGroupSaver(_savedAssetsFolder, logger);
        _assetGroupLoader = new AssetGroupLoader(_assetDataLoader, logger);
    }

    /// <summary>
    /// Whether the given image format (extension) is supported.
    /// </summary>
    /// <param name="fileExtension">Extension of the image file.</param>
    /// <returns><see langword="true"/> if supported. Otherwise <see langword="false"/>.</returns>
    public bool IsFormatValid(string fileExtension)
    {
        return fileExtension is ".jpg" or ".jpeg" or ".png" or ".bmp" or ".tiff" or ".tif" or ".psd" or ".psb";
    }

    /// <summary>
    /// Loads asset with thumbnail from specified filepath.
    /// </summary>
    /// <param name="filePath">Full path to the image file.</param>
    /// <returns>Result of loading. Asset will be empty if loading fails.</returns>
    public Result<Asset> Load(string filePath)
    {
        if (!IsFormatValid(Path.GetExtension(filePath)))
            return new Result<Asset>(false, Asset.Empty, new FileFormatException(new Uri(filePath), "Format is not supported."));

        try
        {
            Asset asset = _assetCreator.Create(filePath);
            return new Result<Asset>(true, asset);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to load asset '{filePath}':\n{ex}");
            return new Result<Asset>(false, Asset.Empty, ex);
        }
    }

    /// <summary>
    /// Loads a collection of assets and its thumbnails from filePaths. If particular asset fails to load it will be skipped.
    /// </summary>
    /// <param name="filePaths">Full paths to the image files.</param>
    /// <returns>Collection of assets.</returns>
    public IEnumerable<Asset> Load(IEnumerable<string> filePaths)
    {
        List<Asset> assets = new();

        foreach (string path in filePaths)
        {
            var loadResult = Load(path);
            if (loadResult.IsSuccessful)
                assets.Add(loadResult.Output);
        }

        return assets;
    }

    /// <summary>
    /// Saves collection of asset groups to file system.
    /// </summary>
    /// <param name="assetGroups">Collection of asset groups.</param>
    /// <returns>Result of the saving.</returns>
    public Result Save(IEnumerable<AssetGroup> assetGroups) => _assetGroupSaver.Save(assetGroups);

    /// <summary>
    /// Asynchronously saves collection of asset groups to file system.
    /// </summary>
    /// <param name="assetGroups">Collection of asset groups.</param>
    /// <returns>Result of the saving.</returns>
    public Task<Result> SaveAsync(IEnumerable<AssetGroup> assetGroups) => _assetGroupSaver.SaveAsync(assetGroups);

    /// <summary>
    /// Asynchronously loads stored asset groups.
    /// </summary>
    /// <returns>Collection of asset groups with all its assets and their thumbnails.</returns>
    public Task<Result<IEnumerable<AssetGroup>>> LoadGroupsAsync() => _assetGroupLoader.LoadGroupsAsync(_savedAssetsFolder);

    /// <summary>
    /// Asynchronously loads stored asset groups and adds them to provided collection one by one.
    /// </summary>
    /// <param name="destination">Collection to populate.</param>
    public Task LoadGroupsToCollectionAsync(IList<AssetGroup> destination) => _assetGroupLoader.LoadGroupsToCollectionAsync(_savedAssetsFolder, destination);
}