using Ookii.Dialogs.Wpf;
using PSQuickAssets.ViewModels;
using PSQuickAssets.Views;

namespace PSQuickAssets.Infrastructure
{
    public class ViewManager
    {
        private MainView _mainView;
        private MainViewModel _mainViewModel;

        public void CreateAndShowMainView()
        {
            _mainViewModel = new MainViewModel(new ImageFileLoader(), new PhotoshopManager());

            _mainView ??= new MainView() { DataContext = _mainViewModel };
            _mainView.Show();
        }

        public void CloseMainView()
        {
            _mainView?.Close();
        }

        public void ChangeWindowVisibility()
        {
            _mainViewModel.IsWindowShowing = !_mainViewModel.IsWindowShowing;
        }

        public static string ShowSelectDirectoryDialog()
        {
            var dialog = new VistaFolderBrowserDialog();

            if (dialog.ShowDialog() is true)
                return dialog.SelectedPath;
            else
                return string.Empty;
        }
    }
}
