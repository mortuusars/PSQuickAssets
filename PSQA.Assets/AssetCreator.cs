using PSQA.Core;

namespace PSQA.Assets;

public interface IAssetCreator
{
    /// <summary>
    /// Creates an asset from specified file path.
    /// </summary>
    /// <param name="filePath">Full path to the file.</param>
    /// <returns>Created asset.</returns>
    /// <exception cref="FileNotFoundException">If file does not exists.</exception>
    /// <exception cref="Exception">If loading filePath fails.</exception>
    Asset Create(string filePath);
}

public class AssetCreator : IAssetCreator
{
    private readonly Dictionary<string, Asset> _loadedAssets = new();

    public Asset Create(string filePath)
    {

        if (_loadedAssets.TryGetValue(filePath, out Asset? loadedAsset))
            return loadedAsset;

        FileInfo file = new(filePath);

        if (!file.Exists)
            throw new FileNotFoundException($"File not found: '{filePath}'");

        Asset asset = new()
        {
            Path = file.FullName,
            FileSize = file.Length
        };

        _loadedAssets.Add(filePath, asset);

        return asset;
    }
}
