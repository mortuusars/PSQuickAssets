using System.Collections.Generic;

namespace PSQuickAssets.Configuration
{
    public interface IConfigLoader
    {
        /// <summary>
        /// Gets config properties for loading.
        /// </summary>
        /// <returns>Config property names and their values.</returns>
        Dictionary<string, object> Load();
    }
}