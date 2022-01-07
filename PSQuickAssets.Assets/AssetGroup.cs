using System.Collections.ObjectModel;
using System.Text;

namespace PSQuickAssets.Assets;

public enum DuplicateHandling
{
    Deny,
    Allow
}

public class AssetGroup
{
    public string Name { get; set; }

    public ObservableCollection<Asset> Assets { get; set; }

    /// <summary>
    /// Indicates that group contents are visible.
    /// </summary>
    public bool IsExpanded { get; set; }

    /// <summary>
    /// Group with empty name and no assets.
    /// </summary>
    public static readonly AssetGroup Empty = new AssetGroup();

    /// <summary>
    /// Initializes new instance of Asset Group with a specified name.
    /// </summary>
    public AssetGroup(string name)
    {
        Name = name;
        Assets = new ObservableCollection<Asset>();
        IsExpanded = true;
    }

    /// <summary>
    /// Initializes new instance of Asset Group.
    /// </summary>
    public AssetGroup() : this(string.Empty) { }

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