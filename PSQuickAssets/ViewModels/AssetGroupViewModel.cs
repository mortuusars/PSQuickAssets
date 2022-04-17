using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using PSQA.Core;
using PSQuickAssets.Assets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;

namespace PSQuickAssets.ViewModels;

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
            if (Group.Name != value)
            {
                Group.Name = value;
                OnPropertyChanged(nameof(Name));
                _assetCatalogSaver.Save();
            }
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
            _assetCatalogSaver.Save();
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

    private readonly AssetRepository _assetRepository;

    internal AssetGroupViewModel(AssetGroup assetGroup, AssetRepository assetRepository)
    {
        Group = assetGroup;
        _assetRepository = assetRepository;

        Group.Assets.CollectionChanged += (s, e) => OnPropertyChanged(nameof(AssetCount));
    }

    /// <summary>
    /// Removes asset from a group.
    /// </summary>
    /// <param name="asset">Asset to remove.</param>
    [ICommand]
    public void RemoveAsset(Asset asset)
    {
        if (Group.Assets.Remove(asset))
            _assetRepository.Save();
    }

    /// <summary>
    /// Adds asset to the group.
    /// </summary>
    /// <param name="asset">Asset to add.</param>
    /// <returns><see langword="true"/> if added successfully, otherwise <see langword="false"/>.</returns>
    public bool AddAsset(Asset asset)
    {
        ArgumentNullException.ThrowIfNull(nameof(asset));

        if (HasAsset(asset.Path))
            return false;

        Group.Assets.Add(asset);
        _assetRepository.Save();
        return true;
    }

    /// <summary>
    /// Adds multiple assets to the group.
    /// </summary>
    /// <param name="assets">Asset collection to add.</param>
    /// <returns>List of assets that were NOT added.</returns>
    public List<Asset> AddAssets(IEnumerable<Asset> assets)
    {
        var notAddedList = new List<Asset>();

        foreach (var asset in assets)
        {
            if (!AddAsset(asset))
                notAddedList.Add(asset);
        }

        if (notAddedList.Count != assets.Count())
            _assetRepository.Save();

        return notAddedList;
    }

    /// <summary>
    /// Checks if asset with the same FILEPATH is already in the group.
    /// </summary>
    /// <returns><see langword="true"/> if asset is in the group. Otherwise <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">If filepath is <see langword="null"/>.</exception>
    public bool HasAsset(string filePath)
    {
        ArgumentNullException.ThrowIfNull(nameof(filePath));
        return Group.Assets.Any(a => a.Path == filePath);
    }
}