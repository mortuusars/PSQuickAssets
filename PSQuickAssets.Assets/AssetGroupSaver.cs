using Serilog;
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
    AssetSavingResult Save(IEnumerable<AssetGroup> assetGroups);
    /// <summary>
    /// Asynchronously saves collection of asset groups.
    /// </summary>
    /// <param name="assetGroups">Collection to save.</param>
    /// <returns>Result of saving.</returns>
    Task<AssetSavingResult> SaveAsync(IEnumerable<AssetGroup> assetGroups);
}

internal class AssetGroupSaver : IAssetSaver
{

    private object _savingLock = new object();
    private readonly string _assetsFolderPath;
    private readonly ILogger _logger;

    public AssetGroupSaver(string assetsFolderPath, ILogger logger)
    {
        _assetsFolderPath = assetsFolderPath;
        _logger = logger;
    }

    public async Task<AssetSavingResult> SaveAsync(IEnumerable<AssetGroup> assetGroups)
    {
        return await Task.Run(() => Save(assetGroups));
    }

    public AssetSavingResult Save(IEnumerable<AssetGroup> assetGroups)
    {
        var options = new JsonSerializerOptions() { WriteIndented = true };
        
        AssetSavingResult result = new(true, "");

        lock (_savingLock)
        {
            if (CreateDirectoryIfNotExists(_assetsFolderPath) is Exception except)
                return new(false, except.Message);

            BackupOldFiles(_assetsFolderPath, Path.Combine(_assetsFolderPath, "backup"));
            DeleteAllFiles(_assetsFolderPath);

            int groupIndex = 0;


            foreach (var group in assetGroups.ToArray())
            {
                try
                {
                    string json = JsonSerializer.Serialize(group, options);
                    string filePath = Path.Combine(_assetsFolderPath, $"{groupIndex++}-{group.Name.GetHashCode()}");
                    File.WriteAllText(filePath, json);
                }
                catch (Exception ex)
                {
                    result.IsSuccessful = false;
                    result.FailedGroups.Add(group, ex);
                    _logger.Error($"[Asset Group Saving] Failed to save AssetGroup '{group.Name}':\n{ex}");
                }
            }

            string groupWord = groupIndex == 1 ? "group" : "groups";
            _logger.Debug($"[Asset Group Saving] Saved {groupIndex++} {groupWord}.");
        }

        if (result.FailedGroups.Count == 0)
            return new AssetSavingResult(true, "");

        return result;
    }

    private void BackupOldFiles(string sourceDirectoryPath, string destinationDirectoryPath)
    {
        if (CreateDirectoryIfNotExists(destinationDirectoryPath) is Exception)
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

    private Exception? CreateDirectoryIfNotExists(string directoryPath)
    {
        try
        {
            Directory.CreateDirectory(directoryPath);
            return null;
        }
        catch (Exception ex)
        {
            _logger.Error($"[Asset Group Saving] Failed to create directory '{Path.GetDirectoryName(directoryPath)}': {ex.Message}");
            return ex;
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

public class AssetSavingResult
{
    public bool IsSuccessful { get; set; }
    public string Message { get; set; }
    public Dictionary<AssetGroup, Exception> FailedGroups { get; } = new();

    public AssetSavingResult(bool isSuccessful, string message)
    {
        IsSuccessful = isSuccessful;
        Message = message;
    }
}