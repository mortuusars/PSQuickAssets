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
