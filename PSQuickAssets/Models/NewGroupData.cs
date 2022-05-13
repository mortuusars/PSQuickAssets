namespace PSQuickAssets.Models;

public class NewGroupData
{
    /// <summary>
    /// Name of the group or <see langword="null"/> if should be generated.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Collection of assets filepaths.
    /// </summary>
    public IEnumerable<string> FilePaths { get; set; } = new List<string>();

    public NewGroupData(string? name, IEnumerable<string> filePaths)
    {
        Name = name;
        FilePaths = filePaths;
    }
}
