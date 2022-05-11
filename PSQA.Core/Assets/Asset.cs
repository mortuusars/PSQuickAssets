namespace PSQA.Core;

public class Asset
{
    /// <summary>
    /// Full path to the file.
    /// </summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// Times that asset has been added to Photoshop.
    /// </summary>
    public int Uses { get; set; }
}
