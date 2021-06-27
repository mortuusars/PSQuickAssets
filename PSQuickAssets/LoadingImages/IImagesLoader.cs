using System.Collections.Generic;
using System.Threading.Tasks;
using PSQuickAssets.Models;

namespace PSQuickAssets
{
    public interface IImagesLoader
    {
        public Task<List<ImageFile>> LoadImagesAsync(string directoryPath);
        public List<ImageFile> LoadImages(string directoryPath);
    }
}
