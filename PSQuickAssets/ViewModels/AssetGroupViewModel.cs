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
                _assetRepository.Save();
            }
        }
    }

    public ObservableCollection<AssetViewModel> Assets { get; } = new();

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
                _assetRepository.Save();
            }
        }
    }

    /// <summary>
    /// Gets the number of assets currently in a group.
    /// </summary>
    public int AssetCount { get => Group.Assets.Count; }

    /// <summary>
    /// Gets the underlying group instance of this viewmodel.
    /// </summary>
    public AssetGroup Group { get; }

    private readonly AssetRepository _assetRepository;

    internal AssetGroupViewModel(AssetGroup assetGroup, AssetRepository assetRepository)
    {
        Group = assetGroup;
        _assetRepository = assetRepository;

        Group.Assets.ForEach(asset => Assets.Add(new AssetViewModel(asset, assetRepository)));
        Assets.CollectionChanged += (s, e) => OnPropertyChanged(nameof(AssetCount));
    }
    
    [ICommand]
    private void ToggleExpanded()
    {
        IsExpanded = !IsExpanded;
    }

    [ICommand]
    public void RemoveAsset(AssetViewModel asset)
    {
        Assets.Remove(asset);
        Group.Assets.Remove(asset.Asset);
        _assetRepository.Save();
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

    private void AddAsset(Asset asset)
    {
        ArgumentNullException.ThrowIfNull(nameof(asset));
        if (!HasAsset(asset.Path))
        {
            Group.Assets.Add(asset);
            Assets.Add(new AssetViewModel(asset, _assetRepository));
            _assetRepository.Save();
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
        return Assets.Any(a => a.FilePath == filePath);
    }
}