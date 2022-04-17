using PSQA.Core;
using PureLib;
using Serilog;
using System.Text;

namespace PSQA.Assets.Repository;

public interface IAssetRepositoryHandler
{
    /// <summary>
    /// Loads collection of asset groups.
    /// </summary>
    Result<IEnumerable<AssetGroup>> Load();
    /// <summary>
    /// Saves collection of asset groups.
    /// </summary>
    /// <param name="assetGroups">Collection to save.</param>
    Task<Result> SaveAsync(IEnumerable<AssetGroup> assetGroups);
}

/// <summary>
/// Allows asset repository to be loaded from and saved to a directory.
/// </summary>
public class DirectoryRepositoryHandler : IAssetRepositoryHandler
{
    private readonly string _fileNamePrefix;
    private readonly Func<string> _fileNamesSuffixProvider;
    private readonly DirectoryInfo _directory;
    private readonly ILogger _logger;

    private readonly SemaphoreSlim _semaphore = new(1, 1);

    /// <summary>
    /// Creates instance of <see cref="DirectoryRepositoryHandler"/> class with control over file name.
    /// </summary>
    /// <param name="directoryPath">Full path to the directory where repository files will be saved.</param>
    /// <exception cref="ArgumentException">If direcoryPath is null or white space.</exception>
    /// <exception cref="ArgumentNullException">If fileNamePrefix or fileNameSuffixProvider is null.</exception>
    /// <exception cref="Exception">If directory could not be loaded.</exception>
    public DirectoryRepositoryHandler(string directoryPath, string fileNamePrefix, Func<string> fileNamesSuffixProvider, ILogger logger)
    {
        if (string.IsNullOrWhiteSpace(directoryPath))
            throw new ArgumentException("Directory path cannot be null or empty.", nameof(directoryPath));
        ArgumentNullException.ThrowIfNull(fileNamePrefix);
        ArgumentNullException.ThrowIfNull(fileNamesSuffixProvider);

        _directory = new DirectoryInfo(directoryPath);
        _fileNamePrefix = fileNamePrefix;
        _fileNamesSuffixProvider = fileNamesSuffixProvider;
        _logger = logger;
    }

    /// <summary>
    /// Creates instance of <see cref="DirectoryRepositoryHandler"/> class.
    /// </summary>
    /// <param name="directoryPath">Full path to the directory where repository files will be saved.</param>
    /// <exception cref="ArgumentException">If direcoryPath is null or white space.</exception>
    /// <exception cref="Exception">If directory could not be loaded.</exception>
    public DirectoryRepositoryHandler(string directoryPath, ILogger logger)
        : this(directoryPath, "Catalog-", () => DateTimeOffset.Now.ToUnixTimeSeconds().ToString(), logger)
    { }

    public Result<IEnumerable<AssetGroup>> Load()
    {
        // Most recent files first
        FileInfo[] files = GetRepositoryFiles(_directory, _fileNamePrefix)
            .OrderByDescending(f => f.LastWriteTime)
            .ToArray();

        if (files.Length == 0)
        {
            // Nothing to load
            _logger.Debug("No repository files were found.");
            return Result.Ok<IEnumerable<AssetGroup>>(Array.Empty<AssetGroup>());
        }

        string foundFilesDesc = files.Aggregate(new StringBuilder(), (sb, item) => sb
            .Append('\n')
            .Append(item.Name)
            .Append(" - ")
            .Append(item.LastWriteTime)).ToString();
        _logger.Debug("{0} repository files was loaded:{1}", files.Length, foundFilesDesc);

        foreach (var file in files)
        {
            _semaphore.Wait();
            try
            {
                string json = File.ReadAllText(file.FullName);
                IEnumerable<AssetGroup> loadedGroups = json.Deserialize<IEnumerable<AssetGroup>>() ?? Array.Empty<AssetGroup>();
                return Result.Ok(loadedGroups);
            }
            catch (Exception ex)
            {
                _logger.Error("Could not load repository file: '{0}'\n{1}.", file.FullName, ex.Message);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        return Result.Fail<IEnumerable<AssetGroup>>("None of the repository files could be loaded.");
    }

    public async Task<Result> SaveAsync(IEnumerable<AssetGroup> assetGroups)
    {
        ArgumentNullException.ThrowIfNull(assetGroups);

        var files = GetRepositoryFiles(_directory, _fileNamePrefix);
        RemoveExcessFiles(files, keepCount: 4);

        string fileName = _fileNamePrefix + _fileNamesSuffixProvider.Invoke();
        string filePath = Path.Combine(_directory.FullName, fileName);

        await _semaphore.WaitAsync();

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
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Removes oldest files leaving some recent ones.
    /// </summary>
    /// <param name="keepCount">How many most files to keep. > 0.</param>
    private bool RemoveExcessFiles(IEnumerable<FileInfo> files, int keepCount)
    {
        ArgumentNullException.ThrowIfNull(files);
        if (keepCount < 1)
            throw new ArgumentOutOfRangeException(nameof(keepCount), "Count should not be less than 1.");

        FileInfo[] filesToRemove = files
            .OrderByDescending(f => f.LastWriteTime)
            .Skip(keepCount)
            .ToArray();

        try
        {
            foreach (var file in filesToRemove)
                file.Delete();

            return true;
        }
        catch (Exception ex)
        {
            _logger.Warning("Catalog directory was not cleaned properly: {0}.", ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Gets the files in a specified directory that start with specified prefix.
    /// </summary>
    /// <returns>List of found files.</returns>
    private IEnumerable<FileInfo> GetRepositoryFiles(DirectoryInfo directory, string fileNamePrefix)
    {
        _logger.Debug("Loading repository files from '{0}'", directory.FullName);

        try
        {
            return directory.GetFiles().Where(f => f.Name.StartsWith(fileNamePrefix));
        }
        catch (Exception ex)
        {
            _logger.Error("Could not load files in repository directory: {0}.", ex.Message);
            return Array.Empty<FileInfo>();
        }
    }
}
