using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using PSQA.Core;
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
    public event EventHandler? GroupChanged;

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
                GroupChanged?.Invoke(this, EventArgs.Empty);
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
            if (Group.IsExpanded != value)
            {
                Group.IsExpanded = value;
                OnPropertyChanged(nameof(IsExpanded));
                GroupChanged?.Invoke(this, EventArgs.Empty);
            }
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

    internal AssetGroupViewModel(AssetGroup assetGroup)
    {
        Group = assetGroup;
        Group.Assets.CollectionChanged += (s, e) =>
        {
            GroupChanged?.Invoke(this, EventArgs.Empty);
            OnPropertyChanged(nameof(AssetCount));
        };
    }

    /// <summary>
    /// Removes asset from a group.
    /// </summary>
    /// <param name="asset">Asset to remove.</param>
    [ICommand]
    public void RemoveAsset(Asset asset) => Group.Assets.Remove(asset);

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
        return true;
    }

    /// <summary>
    /// Adds multiple assets to the group.
    /// </summary>
    /// <param name="assets">Asset collection to add.</param>
    /// <returns>List of assets that were NOT added.</returns>
    public void AddAssets(IEnumerable<Asset> assets)
    {
        foreach (var asset in assets)
            AddAsset(asset);
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