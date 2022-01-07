using MLogger;
using System.IO;
using System.Text.Json;

namespace PSQuickAssets.Assets;

internal interface IAssetSaver
{
    /// <summary>
    /// Saves collection of asset groups.
    /// </summary>
    /// <param name="assetGroups">Collection to save.</param>
    /// <returns>Result of saving.</returns>
    Result Save(IEnumerable<AssetGroup> assetGroups);
    /// <summary>
    /// Asynchronously saves collection of asset groups.
    /// </summary>
    /// <param name="assetGroups">Collection to save.</param>
    /// <returns>Result of saving.</returns>
    Task<Result> SaveAsync(IEnumerable<AssetGroup> assetGroups);
}

internal class AssetSaver : IAssetSaver
{

    private object _savingLock = new object();
    private readonly string _assetsFolderPath;
    private readonly ILogger _logger;

    public AssetSaver(string assetsFolderPath, ILogger logger)
    {
        _assetsFolderPath = assetsFolderPath;
        _logger = logger;
    }

    public async Task<Result> SaveAsync(IEnumerable<AssetGroup> assetGroups)
    {
        return await Task.Run(() => Save(assetGroups));
    }

    public Result Save(IEnumerable<AssetGroup> assetGroups)
    {
        var options = new JsonSerializerOptions() { WriteIndented = true };
        var exceptions = new List<Exception>();

        lock (_savingLock)
        {
            if (!CreateDirectoryIfNotExists(_assetsFolderPath))
                return new Result(false);

            BackupOldFiles(_assetsFolderPath, Path.Combine(_assetsFolderPath, "backup"));
            DeleteAllFiles(_assetsFolderPath);

            int groupIndex = 0;

            AssetGroup[] groups = assetGroups.ToArray();

            foreach (var group in groups)
            {
                try
                {
                    string json = JsonSerializer.Serialize(group, options);
                    string filePath = Path.Combine(_assetsFolderPath, $"{groupIndex++}-{group.Name.GetHashCode()}");
                    File.WriteAllText(filePath, json);
                }
                catch (Exception ex)
                {
                    var exception = new Exception(group.Name, ex);
                    _logger.Error($"[Asset Group Saving] Failed to save AssetGroup '{group.Name}':\n{exception}");
                    exceptions.Add(ex);
                }
            }

            string groupWord = groupIndex == 1 ? "group" : "groups";
            _logger.Info($"[Asset Group Saving] Saved {groupIndex++} {groupWord} successfully.");
        }

        if (exceptions.Count > 0)
        {
            var aggregateException = new AggregateException(exceptions);
            return new Result(false, aggregateException);
        }

        return new Result(true);
    }

    private void BackupOldFiles(string sourceDirectoryPath, string destinationDirectoryPath)
    {
        if (!CreateDirectoryIfNotExists(destinationDirectoryPath))
            return;

        foreach (var file in GetDirectoryFiles(destinationDirectoryPath))
        {
            try { File.Delete(file); }
            catch (Exception ex) { _logger.Error("[Asset Group Saving] Failed delete old backup file: " + ex.Message); }
        }

        foreach (var file in GetDirectoryFiles(sourceDirectoryPath))
        {
            try
            {
                string newFilePath = Path.Combine(destinationDirectoryPath, Path.GetFileName(file));
                File.Copy(file, newFilePath);
            }
            catch (Exception ex) { _logger.Error("[Asset Group Saving] Failed to backup asset group file: " + ex.Message); }
        }
    }

    private void DeleteAllFiles(string directoryPath)
    {
        foreach (var file in GetDirectoryFiles(directoryPath))
        {
            try { File.Delete(file); }
            catch (Exception ex) { _logger.Error("[Asset Group Saving] - Failed to delete old group file: " + ex.Message); }
        }
    }

    private bool CreateDirectoryIfNotExists(string directoryPath)
    {
        try
        {
            Directory.CreateDirectory(directoryPath);
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error($"[Asset Group Saving] Failed to create directory '{Path.GetDirectoryName(directoryPath)}': {ex.Message}");
            return false;
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
            _logger.Error("[Asset Group Saving] Failed to get directory files: " + ex.Message);
            return Array.Empty<string>();
        }
    }
}