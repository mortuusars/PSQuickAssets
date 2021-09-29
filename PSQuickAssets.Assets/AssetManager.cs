using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSQuickAssets.Assets
{
    public class AssetManager
    {
        private List<Asset> _loadedAssets;

        private IAssetLoader _assetLoader;

        public AssetManager()
        {
            _loadedAssets = new List<Asset>();
        }

        public Asset Load(string filePath)
        {

        }
    }
}
