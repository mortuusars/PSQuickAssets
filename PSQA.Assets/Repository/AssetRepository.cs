using PSQA.Core;
using PureLib;
using Serilog;
using System.Collections.ObjectModel;

namespace PSQA.Assets.Repository;

public class AssetRepository
{
    public ObservableCollection<AssetGroup> AssetGroups { get; } = new();

    private readonly IAssetRepositoryHandler _repositoryHandler;
    private readonly ILogger _logger;

    public AssetRepository(IAssetRepositoryHandler repositoryHandler, ILogger logger)
    {
        _repositoryHandler = repositoryHandler;
        _logger = logger;
    }

    public Result Load()
    {
        var loadResult = _repositoryHandler.Load();
        
        if (loadResult.IsFailure)
            return Result.Fail(loadResult.Error);

        foreach (var group in loadResult.Value)
            AssetGroups.Add(group);

        return Result.Ok();
    }

    public Task<Result> SaveAsync() => _repositoryHandler.SaveAsync(AssetGroups);

    public Asset CreateAsset(string filePath)
    {
        try
        {
            return new AssetCreator().Create(filePath);
        }
        catch (Exception ex)
        {
            return new Asset();
        }
    }
}
