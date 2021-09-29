using PSQuickAssets.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PSQuickAssets.Assets
{
    public class AssetAtlas
    {
        private readonly AtlasLoader _atlasLoader;
        private readonly AtlasSaver _atlasSaver;
        private readonly string _saveFilePath;

        internal AssetAtlas(AtlasLoader atlasLoader, AtlasSaver atlasSaver, string saveFilePath)
        {
            _atlasLoader = atlasLoader;
            _atlasSaver = atlasSaver;
            _saveFilePath = saveFilePath;
        }

        internal async Task<StoredGroup[]> LoadAsync()
        {
            string json = await File.ReadAllTextAsync(_saveFilePath);
            return _atlasLoader.Load(json);
        }

        internal async Task SaveAsync(IEnumerable<AssetGroup> assetGroups)
        {
            await _atlasSaver.SaveAsync(assetGroups, _saveFilePath);
        }
    }

    internal class StoredGroup
    {
        public string Name { get; set; }
        public List<string> AssetsPaths { get; set; }
    }

    //internal class AssetFile
    //{
    //    public string ThumbnailPath { get; set; }
    //    public string FilePath { get; set; }
    //}
}
