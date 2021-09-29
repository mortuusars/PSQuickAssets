using PSQuickAssets.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PSQuickAssets.Assets
{
    internal class AtlasSaver
    {
        internal async Task<bool> SaveAsync(IEnumerable<AssetGroup> assetGroups, string filePath)
        {
            List<StoredGroup> groups = new();

            foreach (var inputGroup in assetGroups)
            {
                StoredGroup group = new() { Name = inputGroup.Name, AssetsPaths = new List<string>() };

                foreach (var inputAsset in inputGroup.Assets)
                {
                    group.AssetsPaths.Add(inputAsset.FilePath);
                }

                groups.Add(group);
            }

            string json = JsonSerializer.Serialize(groups, new JsonSerializerOptions() { WriteIndented = true });

            return await WriteFile(json, filePath);
        }

        private async Task<bool> WriteFile(string contents, string filePath)
        {
            try
            {
                await File.WriteAllTextAsync(filePath, contents);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
