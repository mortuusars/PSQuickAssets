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
using WpfScreenHelper;

namespace PSQuickAssets
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : Window
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

        public MainView()
        {
            InitializeComponent();
        }

        // Registering Global hotkey
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // Global Hotkey registering
            IntPtr handle = new WindowInteropHelper(this).Handle;
            HwndSource source = HwndSource.FromHwnd(handle);
            source.AddHook(HwndHook);
            RegisterHotKey(handle, HOTKEY_ID, MOD_CONTROL + MOD_ALT, F8);
        }

        private void OnGlobalHotkeyPressed()
        {
            if (this.Visibility == Visibility.Visible)
                this.Visibility = Visibility.Hidden;
            else
            {
                this.Visibility = Visibility.Visible;
                RecalculateXPosition();
            }
        }

        private void RecalculateXPosition()
        {
            Point mousePos = MouseHelper.MousePosition;
            Screen activeScreen = Screen.FromPoint(mousePos);
            
            if (mousePos.X < 0)
                Left = ((activeScreen.Bounds.Width/2) * -1) - this.ActualWidth/2;
            else
             Left = activeScreen.Bounds.Width/2 - this.ActualWidth/2;
        }
    }
}
