using System.Text;

namespace PSQA.Core;

public class AssetGroup
{
    /// <summary>
    /// Name of the group.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Collection of assets in the group.
    /// </summary>
    public ICollection<Asset> Assets { get; set; } = new List<Asset>();

    /// <summary>
    /// Indicates that group contents are visible.
    /// </summary>
    public bool IsExpanded { get; set; } = true;

    /// <summary>
    /// Prints group name and all of group's assets.
    /// </summary>
    public override string ToString()
    {
        StringBuilder sb = new();
        foreach (Asset asset in Assets)
            sb.Append('\n').Append('\t').Append(asset.Path);

        return $"Group: {{\"{Name}\"\nAssets: {{{sb}\n}}\n}}";
    }
}