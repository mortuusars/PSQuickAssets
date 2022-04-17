using PSQA.Assets;
using PSQA.Core;
using PSQuickAssets.Services;
using PureLib;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PSQuickAssets;

public class AssetRepositoryLoader
{
    private readonly ILogger _logger;

    public AssetRepositoryLoader(ILogger logger)
    {
        _logger = logger;
    }

    public async Task<Result<IEnumerable<AssetGroup>>> LoadAsync(string directoryPath)
    {
        if (string.IsNullOrWhiteSpace(directoryPath))
            throw new ArgumentException("Directory path cannot be null or empty.", nameof(directoryPath));

        DirectoryInfo directory;

        try
        {
            directory = new DirectoryInfo(directoryPath);
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to create DirectoryInfo from path: {0}.\n{1}", directoryPath, ex.Message);
            return Result.Fail<IEnumerable<AssetGroup>>("Failed to create DirectoryInfo from path: " + directoryPath);
        }


        FileInfo[] files;

        try
        {
            // Most recent files first.
            files = directory
                .GetFiles()
                .OrderByDescending(f => f.LastWriteTime)
                .ToArray();

        }
        catch (Exception ex)
        {
            _logger.Error("Could not load files in repository directory: {0}.", ex.Message);
            return Result.Fail<IEnumerable<AssetGroup>>($"Repository could not be loaded: {ex.Message}.");
        }

        // Nothing to load
        if (files.Length == 0)
            return Result.Ok<IEnumerable<AssetGroup>>(Array.Empty<AssetGroup>());

        // Try to load files from most recent. Then backups.
        foreach (var file in files)
        {
            try
            {
                string json = await File.ReadAllTextAsync(file.FullName);
                IEnumerable<AssetGroup> loadedGroups = json.Deserialize<IEnumerable<AssetGroup>>() ?? Array.Empty<AssetGroup>();
                return Result.Ok(loadedGroups);
            }
            catch (Exception ex)
            {
                _logger.Error("Could not load repository file: '{0}'\n{1}.", file.FullName, ex.Message);
            }
        }

        return Result.Fail<IEnumerable<AssetGroup>>("None of the repository files could be loaded.");
    }
}
