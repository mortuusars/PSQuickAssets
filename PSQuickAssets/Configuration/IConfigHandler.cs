using System.Collections.Generic;

namespace PSQuickAssets.Configuration
{
    public interface IConfigHandler
    {
        //T? Load<T>() where T : BaseConfig;
        Dictionary<string, object> Load();
        void Save<T>(T config) where T : ConfigBase;
    }
}
