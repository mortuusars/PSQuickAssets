using System.Text.Json;
using System;

namespace PSQuickAssets.Assets
{
    internal class AtlasLoader
    {
        internal StoredGroup[] Load(string contents)
        {
            try
            {
                return JsonSerializer.Deserialize<StoredGroup[]>(contents) ?? Array.Empty<StoredGroup>();
            }
            catch (Exception)
            {
                return Array.Empty<StoredGroup>();
            }
        }
    }
}