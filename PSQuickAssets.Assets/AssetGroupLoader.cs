using MLogger;
using PSQuickAssets.Assets.Thumbnails;
using System.IO;
using System.Text.Json;

namespace PSQuickAssets.Assets;

/// <summary>
/// Contains functionality related to loading stored asset groups.<br></br>
/// Groups are stored as individual json files.
/// </summary>
internal class AssetGroupLoader
{
    private readonly ThumbnailManager _thumbnailManager;
    private readonly ILogger _logger;

    public AssetGroupLoader(ThumbnailManager thumbnailManager, ILogger logger)
    {
        _thumbnailManager = thumbnailManager;
        _logger = logger;
    }

    /// <summary>
    /// Loads stored Asset Group from specified json file. Thumbnails included.
    /// </summary>
    /// <param name="filePath">Full path to the group json file.</param>
    /// <returns>Result of the loading. Output will be empty if not successful.</returns>
    public Result<AssetGroup> Load(string filePath)
    {
        AssetGroup? group = LoadGroupFromFile(filePath);

        if (group is not null)
        {
            CreateThumbnails(group);
            return new Result<AssetGroup>(true, group);
        }

        return new Result<AssetGroup>(false, AssetGroup.Empty);
    }

    /// <summary>
    /// Asynchronously loads stored Asset Group from specified json file. Thumbnails included.
    /// </summary>
    /// <param name="filePath">Full path to the group json file.</param>
    /// <returns>Result of the loading.</returns>
    public Task<Result<AssetGroup>> LoadAsync(string filePath)
    {
        return Task.Run(() => Load(filePath));
    }

    /// <summary>
    /// Loads all Asset Groups from specified folder. Thumbnails included.
    /// </summary>
    /// <param name="assetGroupsFolder">Full path to the folder.</param>
    /// <returns>Result of the loading with collection of Asset Group or an empty list if none loaded.</returns>
    public Result<IEnumerable<AssetGroup>> LoadGroups(string assetGroupsFolder)
    {
        string[] files = GetDirectoryFiles(assetGroupsFolder);

        List<AssetGroup> groups = new();
        int failedLoads = 0;

        foreach (var file in files)
        {
            var result = Load(file);
            if (result.IsSuccessful)
                groups.Add(result.Output);
            else
                failedLoads++;
        }

        if (failedLoads == assetGroupsFolder.Length)
            return new Result<IEnumerable<AssetGroup>>(false, Array.Empty<AssetGroup>());

        return new Result<IEnumerable<AssetGroup>>(true, groups);
    }

    /// <summary>
    /// Asynchronously loads all Asset Groups from specified folder. Thumbnails included.
    /// </summary>
    /// <param name="assetGroupsFolder">Full path to the folder.</param>
    /// <returns>Result of the loading with collection of Asset Group or an empty list if none loaded.</returns>
    public async Task<Result<IEnumerable<AssetGroup>>> LoadGroupsAsync(string assetGroupsFolder)
    {
        string[] files = GetDirectoryFiles(assetGroupsFolder);

        List<AssetGroup> groups = new();
        int failedLoads = 0;

        foreach (var groupFile in files)
        {
            var result = await LoadAsync(groupFile);
            if (result.IsSuccessful)
                groups.Add(result.Output);
            else
                failedLoads++;
        }

        if (failedLoads == assetGroupsFolder.Length)
            return new Result<IEnumerable<AssetGroup>>(false, Array.Empty<AssetGroup>());

        return new Result<IEnumerable<AssetGroup>>(true, groups);
    }

    /// <summary>
    /// Asynchronously loads all Asset Groups in a specified folder to provided collection. Groups loaded one by one, as loaded.
    /// </summary>
    /// <param name="assetGroupsFolder">Full path to the folder.</param>
    /// <param name="destination">Collection that will be populated with loaded groups.</param>
    public async Task LoadGroupsToCollectionAsync(string assetGroupsFolder, IList<AssetGroup> destination)
    {
        foreach (var groupFile in GetDirectoryFiles(assetGroupsFolder))
        {
            var result = await LoadAsync(groupFile);
            if (result.IsSuccessful)
                destination.Add(result.Output);
        }
    }

    private void CreateThumbnails(AssetGroup group)
    {
        foreach (var asset in group.Assets)
        {
            asset.Thumbnail = _thumbnailManager.Create(asset.Path);
        }
    }

    private AssetGroup? LoadGroupFromFile(string filePath)
    {
        try
        {
            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<AssetGroup>(json);
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to load AssetGroup:\n" + ex);
            return null;
        }
    }

    private string[] GetDirectoryFiles(string directoryPath)
    {
        try
        {
            return Directory.GetFiles(directoryPath);
        }
        catch (Exception ex)
        {
            _logger.Error("[Loading Assset Groups] Getting directory files failed:\n" + ex);
            return Array.Empty<string>();
        }
    }
}