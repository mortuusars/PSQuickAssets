using System.Windows.Controls;

namespace PSQuickAssets
{
    public record FileRecord
    {
        public string FilePath { get; init; }
        public string FileName { get; init; }
    }
}
