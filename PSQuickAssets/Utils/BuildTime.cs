﻿using System;
using System.Globalization;
using System.Reflection;

namespace PSQuickAssets.Utils
{
    internal static class BuildTime
    {
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
}
