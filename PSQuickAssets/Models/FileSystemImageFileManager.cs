using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using PSQuickAssets.Infrastructure;

namespace PSQuickAssets.Models
{
    public class FileSystemImageFileManager : IFileImageFileManager
    {
        public async Task<List<ImageFileInfo>> GetFilesAsync(string directoryPath)
        {
            return await Task.Run(() => GetValidFiles(directoryPath));
        }

        private List<ImageFileInfo> GetValidFiles(string directoryPath)
        {
            List<ImageFileInfo> fileRecords = new List<ImageFileInfo>();
            string[] validTypes = new string[] { ".jpg", ".png" };

            StringFormatter stringFormatter = new StringFormatter();

            foreach (string filePath in GetDirectoryFiles(directoryPath))
            {
                if (Array.Exists(validTypes, type => type == Path.GetExtension(filePath)))
                {
                    fileRecords.Add(new ImageFileInfo()
                    {
                        FilePath = filePath,
                        FileName = Path.GetFileName(filePath),
                        ShortFileName = stringFormatter.CutStart(Path.GetFileName(filePath), 26)
                    });
                }
            }

            return fileRecords;
        }

        private string[] GetDirectoryFiles(string directoryPath)
        {
            if (string.IsNullOrWhiteSpace(directoryPath))
                return Array.Empty<string>();

            try
            {
                return Directory.GetFiles(directoryPath);
            }
            catch (Exception)
            {
                return Array.Empty<string>();
            }
        }

    }
}
