﻿using PSQuickAssets.ViewModels;
using PSQuickAssets.Windows;
using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PSQuickAssets.Update
{
    public class Update
    {
        public async void CheckUpdatesAsync()
        {
            var checkUpdateResult = await CheckVersionFromGithubAsync();

            if (checkUpdateResult.updateAvailable)
                ShowUpdateWindow(App.Version, checkUpdateResult.newVersion, await GetChangelogFromGithub());
        }

        private async Task<string> GetChangelogFromGithub()
        {
            try
            {
                return await ReadStringFromURL("https://raw.githubusercontent.com/mortuusars/PSQuickAssets/master/changelog.md");
            }
            catch (Exception)
            {
                return "";
            }
        }

        private async Task<(bool updateAvailable, Version newVersion)> CheckVersionFromGithubAsync()
        {
            string file;

            try
            {
                file = await ReadStringFromURL("https://raw.githubusercontent.com/mortuusars/PSQuickAssets/master/PSQuickAssets/App.xaml.cs");
            }
            catch (Exception)
            {
                file = "";
            }

            Version ghVersion = GetVersionFromGithubFile(file);

            if (ghVersion > App.Version)
                return (true, ghVersion);

            return (false, null);
        }

        private Version GetVersionFromGithubFile(string file)
        {
            var match = Regex.Match(file, "Version Version .* new Version\\(.*\\);");
            if (match.Success)
            {
                var verNumber = Regex.Match(match.Value, @"\d(\.\d+)*");
                if (verNumber.Success)
                {
                    try
                    {
                        return new Version(verNumber.Value);
                    }
                    catch (Exception)
                    {
                        return new Version();
                    }
                }
            }

            return new Version();
        }

        private async Task<string> ReadStringFromURL(string url)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    return await response.Content.ReadAsStringAsync();
                else
                    return string.Empty;
            }
        }

        private void ShowUpdateWindow(Version currentVersion, Version newVersion, string changelog)
        {
            UpdateWindow updateWindow = new();
            updateWindow.DataContext = new UpdateViewModel(currentVersion, newVersion, changelog);
            updateWindow.Show();
        }
    }
}
