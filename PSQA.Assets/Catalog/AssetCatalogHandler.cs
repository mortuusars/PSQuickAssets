using PSQA.Core;
using PureLib;

namespace PSQA.Assets;

public interface IAssetCatalogHandler
{
    public Task<IEnumerable<AssetGroup>> LoadAsync();
    public Task<Exception?> SaveAsync(IEnumerable<AssetGroup> assetGroups);
}

public class DirectoryAssetCatalogHandler : IAssetCatalogHandler
{
    private readonly DirectoryInfo _directory;
    private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);
    private const string _fileNamePrefix = "Catalog-";
    public DirectoryAssetCatalogHandler(DirectoryInfo directory)
    {
        _directory = directory;
        _directory.Create();
    }

    public async Task<IEnumerable<AssetGroup>> LoadAsync()
    {
        await _semaphoreSlim.WaitAsync();

        try
        {
            // Get catalog files, most recent first.
            var files = _directory.GetFiles()
                .Where(f => f.Name.StartsWith(_fileNamePrefix))
                .OrderByDescending(f => f.LastWriteTime);

            foreach (var file in files)
            {
                try
                {
                    string json = await File.ReadAllTextAsync(file.FullName);
                    return json.Deserialize<IEnumerable<AssetGroup>>() ?? Array.Empty<AssetGroup>();
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }
        finally
        {
            _semaphoreSlim.Release();
        }

        return Array.Empty<AssetGroup>();
    }

    public async Task<Exception?> SaveAsync(IEnumerable<AssetGroup> assetGroups)
    {
        Exception? exception = null;

        await _semaphoreSlim.WaitAsync();

        try
        {
            CleanUpDirectory();
            string fileName = _fileNamePrefix + DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
            string filePath = Path.Combine(_directory.FullName, fileName);
            string json = assetGroups.Serialize();
            await File.WriteAllTextAsync(json, filePath);
        }
        catch (Exception ex)
        {
            exception = ex;
        }
        finally
        {
            _semaphoreSlim.Release();
        }

        return exception;
    }

    private bool CleanUpDirectory()
    {
        try
        {
            var files = _directory.GetFiles()
                .Where(f => f.Name.StartsWith(_fileNamePrefix))
                .OrderBy(f => f.LastWriteTime).ToArray();

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
        catch (Exception)
        {
            return false;
        }
    }
}
