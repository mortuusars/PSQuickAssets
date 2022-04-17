using AsyncAwaitBestPractices;
using PSQA.Core;
using PSQuickAssets.ViewModels;
using PureLib;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PSQuickAssets.Assets;

public class AssetRepository
{
    public ObservableCollection<AssetGroupViewModel> AssetGroups { get; } = new();

    private readonly IAssetRepositoryLoader _assetRepositoryLoader;
    private readonly IAssetRepositorySaver _assetRepositorySaver;

    private readonly ILogger _logger;
    private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

    private readonly string _repositoryDirectoryPath = App.AppName + "/catalog/";
    private static string _repositoryFileNamePrefix = "Catalog-";

    public AssetRepository(ILogger logger)
    {
        _assetRepositoryLoader 
        _assetRepositorySaver = new DirectoryRepositorySaver(_repositoryDirectoryPath, _repositoryFileNamePrefix, logger);
        _logger = logger;
    }

    #region LOADING

    public IEnumerable<AssetGroup> Load()
    {
        return _assetRepositoryLoader.Load();
    }

    public Task<IEnumerable<AssetGroup>> LoadAsync() => _assetRepositoryLoader.LoadAsync();

    #endregion


    #region SAVING
    
    /// <summary>
    /// Saves repository asynchronously, but without awaiting.
    /// </summary>
    public void Save() => SaveAsync().SafeFireAndForget();

    /// <summary>
    /// Saves repository asynchronously. 
    /// </summary>
    /// <returns>Result of the saving.</returns>
    public async Task<Result> SaveAsync()
    {
        IEnumerable<AssetGroup> groups = AssetGroups.Select(g => g.Group);
        string fileName = _repositoryFileNamePrefix + DateTimeOffset.Now.ToUnixTimeSeconds();
        //Result result = Result.Ok();

        await _semaphoreSlim.WaitAsync();
        try
        {
            return await _assetRepositorySaver.SaveAsync(groups, _repositoryDirectoryPath, fileName);
        }
        finally
        {
            _semaphoreSlim.Release();
        }
        //return result;
    } 

    #endregion
}
