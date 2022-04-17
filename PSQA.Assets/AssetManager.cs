using PSQA.Core;
using PureLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSQA.Assets;

public class AssetManager
{
    public AssetFormatValidator FormatValidator { get; set; } = new();

    private readonly IAssetCreator _assetCreator;

    public AssetManager(IAssetCreator assetCreator)
    {
        _assetCreator = assetCreator;
    }

    /// <summary>
    /// Creates asset from filePath.
    /// </summary>
    /// <param name="filePath">Full path to the file.</param>
    /// <returns>Result of creation.</returns>
    /// <exception cref="ArgumentNullException">If filePath is null.</exception>
    public Result<Asset> CreateAsset(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentNullException(nameof(filePath));

        string format = Path.GetExtension(filePath);
        if (FormatValidator.Validate(format).IsFailure)
            return Result.Fail<Asset>($"'{format}' is not a valid format for an asset.");

        try
        {
            return Result.Ok(_assetCreator.Create(filePath));
        }
        catch (Exception ex)
        {
            return Result.Fail<Asset>($"Failed to create asset from path '{filePath}':\n{ex.Message}");
        }
    }
}
