using MLogger;
using PSQuickAssets.Services;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace PSQuickAssets.Update;

/// <summary>
/// Provides functionality to check for a new version by looking at Github repo.
/// </summary>
internal class UpdateChecker
{
    private readonly WindowManager _windowManager;
    private readonly ILogger _logger;

    public UpdateChecker(WindowManager windowManager, ILogger logger)
    {
        _windowManager = windowManager;
        _logger = logger;
    }

    /// <summary>
    /// Checks if update is available by comparing current version with latest version available.
    /// </summary>
    /// <param name="currentVersion">Version of the current application.</param>
    /// <returns></returns>
    public async Task CheckUpdatesAsync(Version currentVersion)
    {
        var githubVersion = await CheckVersionFromGithubAsync();

        if (githubVersion > currentVersion)
        {
            string changelog = await GetChangelogFromGithub();
            _windowManager.ShowUpdateWindow(currentVersion, githubVersion, changelog);
        }
    }

    private async Task<Version> CheckVersionFromGithubAsync()
    {
        try
        {
            string file = await ReadStringFromURL("https://raw.githubusercontent.com/mortuusars/PSQuickAssets/master/PSQuickAssets/PSQuickAssets.csproj");
            return UpdateVersionFinder.GetVersionFromFile(file);
        }
        catch (Exception ex)
        {
            _logger.Error("[Update Checker] Getting version from GitHub failed: " + ex.Message);
            return new Version();
        }
    }

    private async Task<string> GetChangelogFromGithub()
    {
        try { return await ReadStringFromURL("https://raw.githubusercontent.com/mortuusars/PSQuickAssets/master/changelog.md"); }
        catch (Exception ex) { _logger.Error("[Update Checker] Failed to get changelog from github: " + ex.Message); return ""; }
    }

    

    private async Task<string> ReadStringFromURL(string url)
    {
        using (var client = new HttpClient())
        {
            var response = await client.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }
    }
}

internal class UpdateVersionFinder
{
    /// <summary>
    /// Looks for xml version tag and parses that version to the Version object.
    /// </summary>
    /// <param name="input">Input string.</param>
    /// <returns>Parsed version.</returns>
    /// <exception cref="Exception">When something fails.</exception>
    public static Version GetVersionFromFile(string input)
    {
        string versionStartTag = "<Version>";
        int startIndex = input.IndexOf("<Version>") + versionStartTag.Length;

        int endIndex = startIndex;

        while (endIndex < input.Length)
        {
            if (input[endIndex] == '<')
                break;

            endIndex++;
        }

        var versionString = input.Substring(startIndex, endIndex - startIndex);
        Version version = Version.Parse(versionString);
        return version;
    }
}