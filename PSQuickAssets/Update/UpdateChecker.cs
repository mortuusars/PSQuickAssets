using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PSQuickAssets.Update
{
    public class UpdateChecker
    {
        public async Task<(bool updateAvailable, VersionInfo versionInfo)> CheckAsync()
        {
            string json = await GetFileFromWeb();
            VersionInfo versionInfo = DeserializeVersionInfo(json);

            if (new Version(versionInfo.Version) > App.Version)
                return (true, versionInfo);
            else
                return (false, null);
        }

        private VersionInfo DeserializeVersionInfo(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<VersionInfo>(json);
            }
            catch (Exception)
            {
                return new VersionInfo() { Description = string.Empty, Version = "0.0.0" };
            }
        }

        private async Task<string> GetFileFromWeb()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync("https://raw.githubusercontent.com/mortuusars/PhotoshopTimeTracker/master/PSQuickAssets/version.json");
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
