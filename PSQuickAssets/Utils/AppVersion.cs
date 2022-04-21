using System;
using System.Globalization;
using System.Reflection;

namespace PSQuickAssets.Utils;

internal static class AppVersion
{
    /// <summary>
    /// Gets the version from an entry assembly.
    /// </summary>
    public static Version AssemblyVersion { get => GetVersionFromAssembly(); }
    /// <summary>
    /// Gets the lastest build time from an entry assembly.
    /// </summary>
    public static DateTime BuildTime { get => GetBuildTime(); }

    /// <summary>
    /// Gets version number defined in .csproj file.
    /// </summary>
    private static Version GetVersionFromAssembly()
    {
        try
        {
            string info = Assembly.GetEntryAssembly()!.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion;
            return new Version(info.Substring(0, info.IndexOf('+')));
        }
        catch (Exception)
        {
            return new Version("0.0.0");
        }
    }

    /// <summary>
    /// Gets the build time from entry assembly.
    /// </summary>
    private static DateTime GetBuildTime()
    {
        try
        {
            if (Assembly.GetEntryAssembly() is not Assembly assembly)
                return DateTime.MinValue;
            return GetLinkerTime(assembly);
        }
        catch (Exception)
        {
            return DateTime.MinValue;
        }
    }

    /// <summary>
    /// Gets latest build time from the specified assembly.
    /// </summary>
    private static DateTime GetLinkerTime(Assembly assembly)
    {
        const string BuildVersionMetadataPrefix = "+build";

        var attribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
        if (attribute?.InformationalVersion != null)
        {
            var value = attribute.InformationalVersion;
            var index = value.IndexOf(BuildVersionMetadataPrefix);
            if (index > 0)
            {
                value = value[(index + BuildVersionMetadataPrefix.Length)..];
                return DateTime.ParseExact(value, "yyyy-MM-ddTHH:mm:ss:fffZ", CultureInfo.InvariantCulture);
            }
        }

        return DateTime.MinValue;
    }
}