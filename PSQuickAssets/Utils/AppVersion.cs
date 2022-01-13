using System;
using System.Globalization;
using System.Reflection;

namespace PSQuickAssets.Utils;

internal static class AppVersion
{
    /// <summary>
    /// Gets version number defined in .csproj file.
    /// </summary>
    public static string GetVersionFromAssembly()
    {
        try
        {
            string info = Assembly.GetEntryAssembly()!.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion;
            return info.Substring(0, info.IndexOf('+'));
        }
        catch (Exception)
        {
            return "99.99.99";
        }
    }

    /// <summary>
    /// Gets lates build Time.
    /// </summary>
    /// <param name="assembly"></param>
    public static DateTime GetLinkerTime(Assembly assembly)
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

        return default;
    }
}
