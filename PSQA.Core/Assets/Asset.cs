using System.Drawing;
using System.Text.Json.Serialization;

namespace PSQA.Core;

public class Asset
{
    /// <summary>
    /// Full path to the file.
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// Gets or sets size of the image in bytes.
    /// </summary>
    [JsonIgnore]
    public long FileSize { get; set; }

    /// <summary>
    /// Gets name of the file. Includes extension.
    /// </summary>
    [JsonIgnore]
    public string FileName { get => System.IO.Path.GetFileName(Path); }

    /// <summary>
    /// Gets name of the file without extension.
    /// </summary>
    [JsonIgnore]
    public string Name { get => System.IO.Path.GetFileNameWithoutExtension(Path); }

    /// <summary>
    /// Gets format (extension) of the file (including the period ".").
    /// </summary>
    [JsonIgnore]
    public string Format { get => System.IO.Path.GetExtension(Path); }

    /// <summary>
    /// Creates instance with all properties empty.
    /// </summary>
    public Asset() : this(string.Empty) { }

    /// <summary>
    /// Create instance and sets <see cref="Path"/> to specified file path.
    /// </summary>
    /// <param name="filePath"></param>
    public Asset(string filePath)
    {
        Path = filePath;
    }
}
