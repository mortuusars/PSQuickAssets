using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using PropertyChanged;

namespace PSQuickAssets.Views
{
    /// <summary>
    /// Interaction logic for Splash.xaml
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public partial class SplashView : Window
    {
        public bool IsClosing { get; set; }

        public SplashView()
        {
            InitializeComponent();
            //Animate();

            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(200);
            dispatcherTimer.Tick += (s, e) => { IsClosing = true; dispatcherTimer.Stop(); };
            dispatcherTimer.Start();
        }

        private void ContainerAnimation_Completed(object sender, EventArgs e) => this.Close();

        private void Animate()
        {
            var anim = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(500), FillBehavior.Stop);
            Container.BeginAnimation(Grid.OpacityProperty, anim);
        }
    }
}
