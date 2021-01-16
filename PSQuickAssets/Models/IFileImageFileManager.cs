using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSQuickAssets.Models
{
    public interface IFileImageFileManager
    {
        public Task<List<ImageFileInfo>> GetFilesAsync(string directoryPath);
    }
}
