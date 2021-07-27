using Microsoft.WindowsAPICodePack.Dialogs;
using PSQuickAssets.ViewModels;

namespace PSQuickAssets.Infrastructure
{
    public class ViewManager
    {
        private MainView _mainView;
        private MainViewModel _mainViewModel;

        public void CreateAndShowMainView()
        {
            _mainViewModel = new MainViewModel(new ImageFileLoader());

            _mainView ??= new MainView() { DataContext = _mainViewModel };
            _mainView.Show();
        }

        public void CloseMainView()
        {
            _mainView?.Close();
        }

        public static string ShowSelectDirectoryDialog()
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;

            if (ConfigManager.Config.Directories.Count > 0)
                dialog.InitialDirectory = ConfigManager.Config.Directories[ConfigManager.Config.Directories.Count - 1];

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                return dialog.FileName;

            return string.Empty;
        }

        public void ChangeWindowVisibility()
        {
            _mainViewModel.IsWindowShowing = !_mainViewModel.IsWindowShowing;
        }
    }
}
