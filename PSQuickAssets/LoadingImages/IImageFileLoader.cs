using System.Collections.Generic;
using System.Threading.Tasks;
using PSQuickAssets.Models;

namespace PSQuickAssets
{
    public interface IImageFileLoader
    {
        List<ImageFile> Load(string directoryPath, int imageSize, ConstrainTo constrainTo);
        Task<List<ImageFile>> LoadAsync(string directoryPath, int imageSize, ConstrainTo constrainTo);
    }
}