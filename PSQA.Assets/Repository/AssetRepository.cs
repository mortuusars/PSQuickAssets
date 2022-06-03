﻿using PSQA.Core;
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

    public async void Save()
    {
        try
        {
            await SaveAsync();
        }
        catch (Exception ex)
        {
            _logger.Error("Repository saving in async void failed: {0}", ex.Message);
        }
    }

    public Task<Result> SaveAsync() => _repositoryHandler.SaveAsync(AssetGroups);

    public Asset CreateAsset(string filePath)
    {
        try
        {
            return new AssetCreator().Create(filePath);
        }
        catch (Exception)
        {
            return new Asset();
        }
    }

    public AssetGroup AddGroup(string groupName, List<Asset> assets)
    {
        if (string.IsNullOrWhiteSpace(groupName)) throw new ArgumentNullException("groupName", "Name cannot be null or empty.");
        ArgumentNullException.ThrowIfNull(assets);

        AssetGroup group = new()
        {
            Name = groupName,
            Assets = assets
        };

        AssetGroups.Add(group);

        return group;
    }

}
