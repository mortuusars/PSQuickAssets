using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSQuickAssets.ViewModels;

internal class AssetsWindowViewModel
{
    public IConfig Config { get; }

    public AssetsWindowViewModel(IConfig config)
    {
        Config = config;
    }

}
