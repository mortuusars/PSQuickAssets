using PSQA.Core;
using PureLib;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PSQuickAssets;

public interface IAssetRepositorySaver
{
    /// <summary>
    /// Asynchronously saves collection of asset groups.
    /// </summary>
    /// <param name="assetGroups">Source collection.</param>
    /// <returns>Result of saving.</returns>
    Task<Result> SaveAsync(IEnumerable<AssetGroup> assetGroups);
}

public class DirectoryRepositorySaver : IAssetRepositorySaver
{
    private readonly string _directoryPath;
    private readonly string _fileNamePrefix;
    private readonly ILogger _logger;

    public DirectoryRepositorySaver(string directoryPath, string fileNamePrefix, ILogger logger)
    {
        if (string.IsNullOrWhiteSpace(directoryPath))
            throw new ArgumentException("Directory path cannot be null or empty.", nameof(directoryPath));

        if (string.IsNullOrWhiteSpace(fileNamePrefix))
            throw new ArgumentException("Filename cannot be null or empty.", nameof(fileNamePrefix));

        _directoryPath = directoryPath;
        _fileNamePrefix = fileNamePrefix;
        _logger = logger;
    }

    public async Task<Result> SaveAsync(IEnumerable<AssetGroup> assetGroups)
    {
        ArgumentNullException.ThrowIfNull(assetGroups);

        DirectoryInfo directory;

        try
        {
            directory = new DirectoryInfo(_directoryPath);
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to create DirectoryInfo from path: {0}.\n{1}", _directoryPath, ex.Message);
            return Result.Fail("Failed to create DirectoryInfo from path: " + _directoryPath);
        }

        CleanUpDirectory(directory);

        string fileName = _fileNamePrefix + DateTimeOffset.Now.ToUnixTimeSeconds();
        string filePath = Path.Combine(directory.FullName, fileName);

        try
        {
            string json = assetGroups.Serialize();
            await File.WriteAllTextAsync(json, filePath);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.Error("Asset catalog could not be saved:\n{0}", ex);
            return Result.Fail($"Asset catalog could not be saved: {ex.Message}.");
        }
    }

    private bool CleanUpDirectory(DirectoryInfo directory)
    {
        try
        {
            var files = directory
                .GetFiles()
                .Where(f => f.Name.StartsWith(_fileNamePrefix))
                .OrderBy(f => f.LastWriteTime)
                .ToArray();

            int excessFiles = files.Length - 5;

            if (excessFiles > 0)
            {
                foreach (var file in files.Take(excessFiles))
                {
                    file.Delete();
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.Warning("Catalog directory was not cleaned properly: {0}.", ex.Message);
            return false;
        }
    }
}
