﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using WpfScreenHelper;

namespace PSQuickAssets.Views
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
                this.Visibility = Visibility.Collapsed;
            else
            {
                this.Visibility = Visibility.Visible;
                //RecalculatePosition();
            }
        }

        //private void RecalculatePosition()
        //{
        //    Point mousePos = MouseHelper.MousePosition;
        //    Screen activeScreen = Screen.FromPoint(mousePos);

        //    this.MaxHeight = activeScreen.Bounds.Height * 0.7;
            
        //    if (mousePos.X < 0)
        //        Left = ((activeScreen.Bounds.Width/2) * -1) - this.ActualWidth/2;
        //    else
        //        Left = activeScreen.Bounds.Width/2 - this.ActualWidth/2;

        //    Top = (activeScreen.Bounds.Height - (activeScreen.Bounds.Height * 0.1)) - this.ActualHeight;

        //    //TODO: Fix position when clicked from taskbar
        //}

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private enum ResizeDirection { Left = 61441, Right = 61442, Top = 61443, Bottom = 61446, BottomRight = 61448, }

        private bool _isDragging;
        private Point _prevMousePos;

        private void CornerResize_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //_isDragging = true;
            //_prevMousePos = Mouse.GetPosition(this);
            //DragResize(e);

            var hwndSource = PresentationSource.FromVisual((Visual)sender) as HwndSource;
            SendMessage(hwndSource.Handle, 0x112, (IntPtr)ResizeDirection.BottomRight, IntPtr.Zero);
        }

        private void CornerResize_MouseMove(object sender, MouseEventArgs e)
        {
            //DragResize(e);
        }

        //Manual Resizing
        private void DragResize(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                _isDragging = false;
                Mouse.Capture(null);
                return;
            }

            if (_isDragging)
            {
                Mouse.Capture(CornerResize);

                Point currMousePos = Mouse.GetPosition(this);
                if (currMousePos.X == 0 && currMousePos.Y == 0)
                    return;

                this.Width = this.ActualWidth + (currMousePos.X - _prevMousePos.X);
                this.Height = this.ActualHeight + (currMousePos.Y - _prevMousePos.Y);

                _prevMousePos = currMousePos;
            }
        }

        private void CornerResize_MouseEnter(object sender, MouseEventArgs e) => Mouse.OverrideCursor = Cursors.SizeNWSE;

        private void CornerResize_MouseLeave(object sender, MouseEventArgs e) => Mouse.OverrideCursor = Cursors.Arrow;

        private void BG_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
                e.Handled = true;
            }
        }

        private void Scroll_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            e.Handled = false;
            MessageBox.Show(sender.GetType().ToString());
        }
    }
}
