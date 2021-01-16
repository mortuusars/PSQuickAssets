using Microsoft.WindowsAPICodePack.Dialogs;
using PSQuickAssets.Models;
using PSQuickAssets.ViewModels;

namespace PSQuickAssets.Infrastructure
{
    public class ViewManager
    {
        private MainView mainView;

        public void CreateAndShowMainView()
        {
            mainView ??= new MainView() { DataContext = new MainViewModel(new FileSystemImageFileManager()) };
            mainView.Show();
        }

        public void CloseMainView()
        {
            mainView?.Close();
        }

        public static string ShowSelectDirectoryDialog()
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.InitialDirectory = ConfigManager.Config.Directory;

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                return dialog.FileName;

            return string.Empty;
        }
    }
}
