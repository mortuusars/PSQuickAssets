using System.Windows.Input;

namespace PSQuickAssets.ViewModels
{
    public class TaskBarViewModel
    {
        public ICommand ShowWindowCommand { get; }
        public ICommand ExitCommand { get; }

        public TaskBarViewModel()
        {
            ShowWindowCommand = new RelayCommand(_ => App.ViewManager.ChangeWindowVisibility());
            ExitCommand = new RelayCommand(_ => App.Current.Shutdown());
        }
    }
}
