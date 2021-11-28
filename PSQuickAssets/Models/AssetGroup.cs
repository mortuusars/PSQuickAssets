using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace PSQuickAssets.Models;

public class AssetGroup : ObservableObject
{
    /// <summary>
    /// Occurs at any change in the group. E.g adding/removing assets, renaming, etc.
    /// </summary>
    public event Action? GroupChanged;

    /// <summary>
    /// Name of the group.
    /// </summary>
    public string Name { get; private set; }
    /// <summary>
    /// Collection of assets in the group.
    /// </summary>
    public ObservableCollection<Asset> Assets { get; }

    /// <summary>
    /// Initializes Asset Group.
    /// </summary>
    /// <param name="name">Name of the group. Cannot be <see langword="null"/>.</param>
    /// <exception cref="ArgumentNullException">Thrown when name is null.</exception>
    public AssetGroup(string name)
    {
        if (name is null)
            throw new ArgumentNullException(nameof(name));

        Name = name;
        Assets = new ObservableCollection<Asset>();

        Assets.CollectionChanged += (_, _) => GroupChanged?.Invoke();
    }

    /// <summary>
    /// Adds asset to the group.
    /// </summary>
    /// <param name="asset">Asset to add.</param>
    /// <param name="duplicateHandling">Specify how the duplicates should be handled. If set to Deny - asset will not be added if it is already in the group.</param>
    /// <returns><see langword="true"/> if added successfully, otherwise <see langword="false"/>.</returns>
    public bool AddAsset(Asset asset, DuplicateHandling duplicateHandling)
    {
        if (duplicateHandling is DuplicateHandling.Deny && HasAsset(asset))
            return false;

        Assets.Add(asset);
        GroupChanged?.Invoke();
        return true;
    }

    /// <summary>
    /// Adds multiple assets to the group.
    /// </summary>
    /// <param name="assets">Asset collection to add.</param>
    /// <param name="duplicateHandling">Specify how the duplicates should be handled. If set to Deny - asset will not be added if it is already in the group.</param>
    /// <returns>List of assets that were NOT added.</returns>
    public List<Asset> AddMultipleAssets(IEnumerable<Asset> assets, DuplicateHandling duplicateHandling)
    {
        var notAddedList = new List<Asset>();

        foreach (var asset in assets)
        {
            if (duplicateHandling is DuplicateHandling.Allow || !HasAsset(asset))
                Assets.Add(asset);
            else
                notAddedList.Add(asset);
        }

        GroupChanged?.Invoke();
        return notAddedList;
    }

    /// <summary>
    /// Checks if asset with the same FILEPATH is already in the group.
    /// </summary>
    /// <returns><see langword="true"/> if asset is in the group. Otherwise <see langword="false"/>.</returns>
    public bool HasAsset(Asset asset)
    {
        return Assets.Any(a => a.FilePath == asset.FilePath);
    }

    /// <summary>
    /// Renames the group.
    /// </summary>
    /// <param name="name">New name for a group. Cannot be null.</param>
    /// <returns><see langword="true"/> if renames successfully. Otherwise <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">When name is null.</exception>
    public bool Rename(string name)
    {
        if (name is null)
            throw new ArgumentNullException(nameof(name));

        if (Name.Equals(name))
            return false;

        Name = name;
        OnPropertyChanged(nameof(Name));
        GroupChanged?.Invoke();
        return true;
    }

    /// <summary>
    /// Prints group name and all of group's assets.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        StringBuilder sb = new();
        foreach (Asset asset in Assets)
            sb.Append('\n').Append('\t').Append(asset.FileName);

        return $"Group: {{\"{Name}\"\nAssets: {{{sb}\n}}\n}}";
    }
}

public enum DuplicateHandling
{
    Deny,
    Allow
}