using System.Collections.ObjectModel;
using System.Text;

namespace PSQuickAssets.Assets;

public class AssetGroup
{
    /// <summary>
    /// Name of the group.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Collection of assets in the group.
    /// </summary>
    public ObservableCollection<Asset> Assets { get; set; }

    /// <summary>
    /// Indicates that group contents are visible.
    /// </summary>
    public bool IsExpanded { get; set; }

    /// <summary>
    /// Initializes new instance of Asset Group.
    /// </summary>
    /// <param name="name">Name of the group.</param>
    /// <param name="assets">Asset collection.</param>
    /// <param name="isExpanded">Indicates whether the group contents is expanded.</param>
    public AssetGroup(string name, ObservableCollection<Asset> assets, bool isExpanded)
    {
        Name = name;
        Assets = assets;
        IsExpanded = isExpanded;
    }

    /// <summary>
    /// Initializes new instance of Asset Group.
    /// </summary>
    public AssetGroup()
    {
        Name = string.Empty;
        Assets = new ObservableCollection<Asset>();
        IsExpanded = true;
    }

    /// <summary>
    /// Initializes new instance of Asset Group with a specified name.
    /// </summary>
    public AssetGroup(string name) : this()
    {
        Name = name;
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