using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using PSQA.Assets.Repository;
using PSQA.Core;
using System.ComponentModel;
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

    private readonly AssetRepository _assetRepository;

    internal AssetGroupViewModel(AssetGroup assetGroup, AssetRepository assetRepository)
    {
        Group = assetGroup;
        Group.Assets.CollectionChanged += (s, e) =>
        {
            GroupChanged?.Invoke(this, EventArgs.Empty);
            OnPropertyChanged(nameof(AssetCount));
        };
        _assetRepository = assetRepository;
    }

    [ICommand]
    private void ToggleExpanded()
    {
        IsExpanded = !IsExpanded;
    }

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

    [ICommand]
    public void AddAssets(IEnumerable<string>? filesPaths)
    {
        if (filesPaths is null)
            return;

        foreach (var path in filesPaths)
        {
            if (_assetRepository.CreateAsset(path) is Asset asset && asset.Path.Length != 0)
                AddAsset(asset);
        }
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