using System.Diagnostics;
using System.IO;
using System.Text.Json;
using Ookii.Dialogs.Wpf;
using PSQuickAssets.ViewModels;
using PSQuickAssets.Views;
using PSQuickAssets.Views.State;

namespace PSQuickAssets.Infrastructure
{
    public class ViewManager
    {
        private MainView _mainView;
        private MainViewModel _mainViewModel;
        private const string _MAIN_VIEW_STATE_FILE = "state.json";

        public void CreateAndShowMainView()
        {
            _mainViewModel = new MainViewModel(new ImageFileLoader(), new PhotoshopManager());

            _mainView ??= new MainView() { DataContext = _mainViewModel };
            var state = ReadMainViewState();
            _mainView.Left = state.Left;
            _mainView.Top = state.Top;
            _mainView.Width = state.Width;
            _mainView.Height = state.Height;
            _mainView.Show();
        }

        public void CloseMainView()
        {
            _mainView?.Close();
        }

        public void SaveMainViewState()
        {
            var state = new MainViewState()
            {
                Left = _mainView.Left,
                Top = _mainView.Top,
                Width = _mainView.ActualWidth,
                Height = _mainView.ActualHeight,
            };

            var json = JsonSerializer.Serialize(state, new JsonSerializerOptions() { WriteIndented = true } );

            try
            {
                File.WriteAllText(_MAIN_VIEW_STATE_FILE, json);
            }
            catch (System.Exception)
            {
                Debug.WriteLine("Failed to save window state.");
            }
        }

        private MainViewState ReadMainViewState()
        {
            try
            {
                var json = File.ReadAllText(_MAIN_VIEW_STATE_FILE);
                return JsonSerializer.Deserialize<MainViewState>(json);
            }
            catch (System.Exception)
            {
                return new MainViewState()
                {
                    Left = (WpfScreenHelper.Screen.PrimaryScreen.Bounds.Right / 2) - 200,
                    Top = (WpfScreenHelper.Screen.PrimaryScreen.Bounds.Bottom / 2) - 200,
                    Width = 600,
                    Height = 500
                };
            }
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
