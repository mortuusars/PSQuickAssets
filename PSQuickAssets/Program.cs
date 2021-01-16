using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PSQuickAssets.Infrastructure;

namespace PSQuickAssets
{
    public class Program
    {
        public static ViewManager ViewManager { get; private set; }

        public void Startup()
        {
            ViewManager = new ViewManager();
            ViewManager.CreateAndShowMainView();
        }

    }
}
