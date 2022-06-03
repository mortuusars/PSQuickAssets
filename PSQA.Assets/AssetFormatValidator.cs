using PSQA.Core;
using PureLib;

namespace PSQA.Assets;

/// <summary>
/// Validates format (extension) of the file to determine if it is valid asset.
/// </summary>
public class AssetFormatValidator
{
    /// <summary>
    /// Collection of valid formats.
    /// </summary>
    public virtual IEnumerable<string> ValidFormats { get; } = new string[]
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".bmp",
        ".tiff",
        ".tif",
        ".psd",
        ".psb"
    };

    /// <summary>
    /// Validates specified format to determine if it is supported.
    /// </summary>
    /// <param name="format">Extension of the file. Can be with leading dot (.jpg) or witout it (jpg). Whitespace will be trimmed.</param>
    /// <returns>Result of the validation.</returns>
    public virtual Result Validate(string format)
    {
        ArgumentNullException.ThrowIfNull(format);

        if (string.IsNullOrWhiteSpace(format))
            return Result.Fail("Empty string or whitespace is not a valid format");

        format = format.Trim();
        if (!format.StartsWith('.'))
            format = "." + format;

        foreach (var validFormat in ValidFormats)
        {
            if (validFormat.Equals(format, StringComparison.InvariantCultureIgnoreCase))
                return Result.Ok();
        }

        return Result.Fail($"'{format}' is not valid.");
    }
}
