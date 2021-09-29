using System.Text.Json;

namespace PSQuickAssets.Assets
{
    internal class AtlasLoader
    {
        internal StoredGroup[] Load(string contents)
        {
            return JsonSerializer.Deserialize<StoredGroup[]>(contents);
        }
    }
}