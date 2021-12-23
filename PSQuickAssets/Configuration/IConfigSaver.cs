using System.Collections.Generic;

namespace PSQuickAssets.Configuration
{
    public interface IConfigSaver
    {
        /// <summary>
        /// Saves the config.
        /// </summary>
        /// <typeparam name="T">Type of a config.</typeparam>
        /// <param name="config">Instance of a config that would be saved.</param>
        void Save<T>(T config) where T : ConfigBase;
    }
}
