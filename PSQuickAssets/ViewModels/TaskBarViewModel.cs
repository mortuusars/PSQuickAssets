using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PSQuickAssets.ViewModels
{
    public class TaskBarViewModel
    {
        public ICommand ExitCommand { get; }

        public TaskBarViewModel()
        {
            ExitCommand = new RelayCommand(_ => App.Current.Shutdown());
        }
    }
}
