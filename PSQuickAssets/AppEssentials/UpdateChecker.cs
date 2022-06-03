using PSQuickAssets.Services;
using Serilog;
using System.Net.Http;
using System.Threading.Tasks;

namespace PSQuickAssets;

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
    public async Task CheckUpdatesAsync(Version currentVersion)
    {
        if (currentVersion == new Version("0.0.0"))
        {
            _logger.Warning("[{0}] Updates would not be checked because current version is unknown.", this.GetType().Name);
            return;
        }

        var githubVersion = await CheckVersionFromGithubAsync();

        if (githubVersion > currentVersion)
        {
            _logger.Information("[{0}] New version is found: {1}. Current version: {2}", this.GetType().Name, githubVersion, currentVersion);
            
            // Not used. Changelog web page is opened from ui.
            //string changelog = await GetChangelogFromGithub();
            
            _windowManager.ShowUpdateWindow(currentVersion, githubVersion);
        }
    }

    private async Task<Version> CheckVersionFromGithubAsync()
    {
        _logger.Debug("[{0}] Checking github repository for a new version...", this.GetType().Name);
        const string url = "https://raw.githubusercontent.com/mortuusars/PSQuickAssets/master/PSQuickAssets/PSQuickAssets.csproj";

        try
        {
            string file = await ReadStringFromURL(url);
            return CsProjVersionParser.Parse(file);
        }
        catch (Exception ex)
        {
            _logger.Error("[{0}] Getting version from '{1}' failed: {2}.", this.GetType().Name, url, ex.Message);
            return new Version();
        }
    }

    private async Task<string> GetChangelogFromGithub()
    {
        _logger.Debug("[{0}] Getting changelog file...", this.GetType().Name);
        const string url = "https://raw.githubusercontent.com/mortuusars/PSQuickAssets/master/changelog.md";
        try
        {
            return await ReadStringFromURL(url);
        }
        catch (Exception ex)
        {
            _logger.Error("[{0}] Failed to get changelog from '{1}': {2}", this.GetType().Name, url, ex.Message);
            return string.Empty;
        }
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

internal static class CsProjVersionParser
{
    /// <summary>
    /// Looks for xml version tag and parses that version to the Version object.
    /// </summary>
    /// <param name="input">Input string.</param>
    /// <returns>Parsed version.</returns>
    /// <exception cref="Exception">When something fails.</exception>
    public static Version Parse(string input)
    {
        try
        {
            const string versionStartTag = "<Version>";
            int startIndex = input.IndexOf(versionStartTag) + versionStartTag.Length;

            int endIndex = startIndex;

            while (endIndex < input.Length)
            {
                if (input[endIndex] == '<')
                    break;

                endIndex++;
            }

            var versionString = input.Substring(startIndex, endIndex - startIndex);
            return Version.Parse(versionString);
        }
        catch (Exception)
        {
            return new Version("0.0.0");
        }
    }
}