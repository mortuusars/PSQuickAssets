using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace PSQuickAssets
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Global Hotkey

        //Modifiers:
        private const uint MOD_NONE = 0x0000; //[NONE]
        private const uint MOD_ALT = 0x0001; //ALT
        private const uint MOD_CONTROL = 0x0002; //CTRL
        private const uint MOD_SHIFT = 0x0004; //SHIFT

        //Keys
        private const uint HOME = 0x24;
        private const uint F8 = 0x77;

        private const int HOTKEY_ID = 1;


        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);


        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    switch (wParam.ToInt32())
                    {
                        case HOTKEY_ID:
                            int vkey = (((int)lParam >> 16) & 0xFFFF);
                            if (vkey == F8)
                            {
                                OnGlobalHotkeyPressed();
                            }
                            handled = true;
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        #endregion


        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        readonly string[] validTypes = new string[] { ".jpg", ".png" };

        public MainWindow()
        {
            ToolTipService.InitialShowDelayProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(800));

            InitializeComponent();

            var folderPath = ConfigManager.GetFolder();
            ImageList.ItemsSource = GetFilesList(folderPath);

            ImageList.SelectionChanged += ImageList_SelectionChanged;
        }

        public ObservableCollection<FileRecord> GetFilesList(string folderPath)
        {
            ObservableCollection<FileRecord> fileRecords = new ObservableCollection<FileRecord>();
            string[] files = Array.Empty<string>();

            try
            {
                files = Directory.GetFiles(folderPath);
            }
            catch (Exception)
            {
                return fileRecords;
            }

            foreach (var file in files)
            {
                if (Array.Exists(validTypes, type => type == Path.GetExtension(file)))
                {
                    fileRecords.Add(new FileRecord()
                    {
                        FilePath = file,
                        FileName = Path.GetFileName(file)
                    });
                }
            }

            return fileRecords;
        }

        private void SetItemsSource(IEnumerable list)
        {
            ImageList.ItemsSource = list;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // Global Hotkey registering
            IntPtr handle = new WindowInteropHelper(this).Handle;
            HwndSource source = HwndSource.FromHwnd(handle);
            source.AddHook(HwndHook);
            RegisterHotKey(handle, HOTKEY_ID, MOD_CONTROL + MOD_ALT, F8);

            this.Visibility = Visibility.Hidden;
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
                this.Visibility = Visibility.Hidden;
        }

        private void OnGlobalHotkeyPressed()
        {
            if (this.Visibility == Visibility.Visible)
                this.Visibility = Visibility.Hidden;
            else
            {
                //this.Top += this.Height / 2;
                this.Visibility = Visibility.Visible;
            }
                
        }

        //private bool IsPhotoshopOnForeground()
        //{
        //    IntPtr handle = GetForegroundWindow();

        //    foreach (var process in Process.GetProcesses())
        //    {
        //        if (process.Handle == handle)
        //            return true;
        //    }

        //    return false;
        //}

        private void ImageList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ImageList.SelectedItem is not FileRecord record)
                return;

            PlaceClickedImage(record.FilePath);
            ImageList.SelectedItem = null;
        }

        private void PlaceClickedImage(string filePath)
        {
            this.Visibility = Visibility.Hidden;

            bool isSuccessful = MainService.PlaceImage(filePath);

            if (isSuccessful)
            {
                // Bring PS to foreground.
                Process[] proc = Process.GetProcessesByName("photoshop");

                if (proc.Length > 0)
                    SetForegroundWindow(proc[0].MainWindowHandle);
            }
            else
            {
                this.Visibility = Visibility.Visible;
            }
        }

        private void Close_Down(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true;
            this.Visibility = Visibility.Hidden;
        }

        private void ChangeFolder_Down(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.InitialDirectory = ConfigManager.Config.Folder;

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                SetItemsSource(GetFilesList(dialog.FileName));
                ConfigManager.Config = new Config() { Folder = dialog.FileName };
                ConfigManager.Write();
            }
        }
    }
}
