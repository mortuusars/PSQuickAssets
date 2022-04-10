using AsyncAwaitBestPractices;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using PSQuickAssets.Assets;
using PSQuickAssets.Services;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;

namespace PSQuickAssets.ViewModels;

/// <summary>
/// Describes how duplicates should be handled.
/// </summary>
public enum DuplicateHandling
{
    Deny,
    Allow
}


[INotifyPropertyChanged]
public partial class AssetGroupViewModel
{
    /// <summary>
    /// Gets or sets name of the group.
    /// </summary>
    public string Name
    {
        get => Group.Name;
        set
        {
            if (Group.Name == value)
                return;

            _logger.Information($"[Group] Renaming group '{Name}' to '{value}'");
            Group.Name = value;
            _assetGroupHandler.SaveGroupsAsync().SafeFireAndForget();
            OnPropertyChanged(nameof(Name));
        }
    }

    /// <summary>
    /// Gets the <see cref="ICollectionView"/> of the assets in a group.
    /// </summary>
    public ICollectionView Assets { get => CollectionViewSource.GetDefaultView(Group.Assets); }

    /// <summary>
    /// Gets or sets the property that indicates that the group contents should be visible.
    /// </summary>
    public bool IsExpanded
    {
        get => Group.IsExpanded;
        set
        {
            Group.IsExpanded = value;
            _assetGroupHandler.SaveGroupsAsync().SafeFireAndForget();
            OnPropertyChanged(nameof(IsExpanded));
        }
    }

    /// <summary>
    /// Gets the number of assets currently in a group.
    /// </summary>
    public int AssetCount { get => Group.Assets.Count; }

    /// <summary>
    /// Gets the group instance of this viewmodel.
    /// </summary>
    public AssetGroup Group { get; }

    public ICommand RemoveAssetCommand { get; }

    
    private readonly AssetGroupHandler _assetGroupHandler;
    private readonly ILogger _logger;

    internal AssetGroupViewModel(AssetGroup assetGroup, AssetGroupHandler assetGroupHandler, ILogger logger)
    {
        Group = assetGroup;
        _assetGroupHandler = assetGroupHandler;
        _logger = logger;

        RemoveAssetCommand = new RelayCommand<Asset>(a => RemoveAsset(a));

        Group.Assets.CollectionChanged += (s, e) => OnPropertyChanged(nameof(AssetCount));
    }

    /// <summary>
    /// Adds asset to the group.
    /// </summary>
    /// <param name="asset">Asset to add.</param>
    /// <param name="duplicateHandling">Specify how the duplicates should be handled. If set to Deny - asset will not be added if it is already in the group.</param>
    /// <returns><see langword="true"/> if added successfully, otherwise <see langword="false"/>.</returns>
    public bool AddAsset(Asset asset, DuplicateHandling duplicateHandling)
    {
        if (duplicateHandling is DuplicateHandling.Deny && HasAsset(asset.Path))
            return false;

        Group.Assets.Add(asset);
        _logger.Information($"[Group] Added {asset.FileName} to group '{Name}'");
        _assetGroupHandler.SaveGroupsAsync().SafeFireAndForget();
        return true;
    }

    /// <summary>
    /// Adds multiple assets to the group.
    /// </summary>
    /// <param name="assets">Asset collection to add.</param>
    /// <param name="duplicateHandling">Specify how the duplicates should be handled. If set to Deny - asset will not be added if it is already in the group.</param>
    /// <returns>List of assets that were NOT added.</returns>
    public List<Asset> AddAssets(IEnumerable<Asset> assets, DuplicateHandling duplicateHandling)
    {
        var notAddedList = new List<Asset>();

        foreach (var asset in assets)
        {
            if (!AddAsset(asset, duplicateHandling))
                notAddedList.Add(asset);
        }

        _logger.Information($"[Group] Added {assets.Count() - notAddedList.Count} assets to group '{Name}'");
        _assetGroupHandler.SaveGroupsAsync().SafeFireAndForget();
        return notAddedList;
    }

    /// <summary>
    /// Removes asset from a group.
    /// </summary>
    /// <param name="asset">Asset to remove.</param>
    /// <returns><see langword="true"/> if successfully removed. Otherwise <see langword="false"/>.</returns>
    public bool RemoveAsset(Asset? asset)
    {
        bool result = asset is not null && Group.Assets.Remove(asset);
        if (result)
        {
            _logger.Information($"[Group] Removed Asset '{asset!.FileName}' from group '{Name}'");
            _assetGroupHandler.SaveGroupsAsync().SafeFireAndForget();
        }
        return result;
    }

    /// <summary>
    /// Checks if asset with the same FILEPATH is already in the group.
    /// </summary>
    /// <returns><see langword="true"/> if asset is in the group. Otherwise <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">If filepath is <see langword="null"/>.</exception>
    public bool HasAsset(string filePath)
    {
        if (filePath is null)
            throw new ArgumentNullException(nameof(filePath));

        return Group.Assets.Any(a => a.Path == filePath);
    }
}