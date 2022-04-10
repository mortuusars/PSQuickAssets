using Serilog;
using System.IO;
using System.Text.Json;

namespace PSQuickAssets.Assets;

/// <summary>
/// Contains functionality related to loading stored asset groups.<br></br>
/// Groups are stored as individual json files.
/// </summary>
internal class AssetGroupLoader
{
    private readonly AssetDataLoader _assetDataLoader;
    private readonly ILogger _logger;

    public AssetGroupLoader(AssetDataLoader assetDataLoader, ILogger logger)
    {
        _assetDataLoader = assetDataLoader;
        _logger = logger;
    }

    /// <summary>
    /// Asynchronously loads stored Asset Group from specified json file. Thumbnails included.
    /// </summary>
    /// <param name="filePath">Full path to the group json file.</param>
    /// <returns>Result of the loading. Output will be empty if not successful.</returns>
    public async Task<Result<AssetGroup>> LoadGroupAsync(string filePath)
    {
        AssetGroup? group = await LoadGroupFromFile(filePath);

        if (group is null)
            return new Result<AssetGroup>(false, new AssetGroup());

        if (group.Assets.Count == 0)
        {
            _logger.Warning($"[Assset Group Loading] Group '{group.Name}' was loaded, but contains no assets.");
            return new Result<AssetGroup>(false, group);
        }

        List<Asset> failedAssets = new();

        foreach (var asset in group.Assets)
        {
            try
            {
                if (!File.Exists(asset.Path))
                {
                    _logger.Warning($"[Assset Group Loading] Cannot load asset '{asset.Path}'. File does not exist. Asset will be removed.");
                    failedAssets.Add(asset);
                }
            }
            catch (Exception ex) { _logger.Error($"[Assset Group Loading] Failed to check if file exists: {ex.Message}"); }

            await Task.Run(() => _assetDataLoader.LoadData(asset));
        }

        foreach (var asset in failedAssets)
            group.Assets.Remove(asset);

        return new Result<AssetGroup>(true, group);
    }

    public async IAsyncEnumerable<AssetGroup?> LoadStoredGroupsAsync(string assetGroupsFolder)
    {
        string[] files = GetDirectoryFiles(assetGroupsFolder);

        if (files.Length == 0)
            yield break;

        foreach (var groupFile in files)
        {
            AssetGroup? group = await LoadGroupFromFile(groupFile);

            if (group is null)
                continue;

            for (int i = group.Assets.Count - 1; i >= 0; i--)
            {
                Asset asset = group.Assets[i];

                if (File.Exists(asset.Path))
                    await Task.Run(() => _assetDataLoader.LoadData(asset)); // Loads thumbnail and other asset info
                else
                {
                    _logger.Warning($"[Assset Group Loading] Cannot load asset '{asset.Path}'. File does not exist. Asset will be removed.");
                    group.Assets.Remove(asset);
                }
            }

            yield return group;
        }
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
            var result = await LoadGroupAsync(groupFile);
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
            var result = await LoadGroupAsync(groupFile);
            if (result.IsSuccessful)
                destination.Add(result.Output);
        }
    }

    /// <summary>
    /// Tries to open asset group json file and deserialize it to <see cref="AssetGroup"/>.<br></br><br></br>
    /// Loaded asset group has only essential data (e.g. Asset has only Path to the file)<br></br>
    /// Other info (Size, Dimensions, etc..) should be loaded separately.
    /// </summary>
    /// <param name="filePath">Path to the json file.</param>
    /// <returns>Deserialized <see cref="AssetGroup"/> or <see langword="null"/> if failed.</returns>
    private async Task<AssetGroup?> LoadGroupFromFile(string filePath)
    {
        try
        {
            using var stream = File.OpenRead(filePath);
            return await JsonSerializer.DeserializeAsync<AssetGroup>(stream);
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to load AssetGroup:\n" + ex);
            return null;
        }

        try
        {
            using var stream = File.OpenRead(filePath);
            return await JsonSerializer.DeserializeAsync<AssetGroup>(stream);
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to load AssetGroup:\n" + ex);
            return null;
        }
    }

    /// <summary>
    /// Safely gets files in a specified directory.
    /// </summary>
    /// <returns>Array of files in that directory, or empty array if failed or folder is empty.</returns>
    private string[] GetDirectoryFiles(string directoryPath)
    {
        try
        {
            return Directory.GetFiles(directoryPath);
        }
        catch (Exception ex)
        {
            _logger.Error("[Assset Group Loading] Getting directory files failed:\n" + ex);
            return Array.Empty<string>();
        }
    }
}