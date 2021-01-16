using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAPICodePack.Dialogs;

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

            foreach (string filePath in GetDirectoryFiles(directoryPath))
            {
                if (Array.Exists(validTypes, type => type == Path.GetExtension(filePath)))
                {
                    fileRecords.Add(new ImageFileInfo()
                    {
                        FilePath = filePath,
                        FileName = Path.GetFileName(filePath)
                    });
                }
            }

            return fileRecords;
        }

        private string[] GetDirectoryFiles(string directoryPath)
        {
            if (string.IsNullOrWhiteSpace(directoryPath))
                return Array.Empty<string>();

            return Directory.GetFiles(directoryPath);

            //try
            //{
            //    return Directory.GetFiles(directoryPath);
            //}
            //catch (Exception)
            //{
            //    return Array.Empty<string>();
            //}
        }

    }
}
