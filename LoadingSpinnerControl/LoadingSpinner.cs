using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace LoadingSpinnerControl
{
    public class LoadingSpinner : Control
    {
        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register(nameof(IsLoading), typeof(bool), typeof(LoadingSpinner), new PropertyMetadata(false, OnIsLoadingChanged));

        private static void OnIsLoadingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LoadingSpinner spinner)
            {
                var template = spinner.Template;
                var ellipse = template.FindName("Ell", spinner) as Ellipse;

                if (ellipse is null)
                    return;

                var storyboard = (Storyboard)spinner.FindResource("Spinning");

                if ((bool)e.NewValue)
                {
                    ellipse.BeginStoryboard(storyboard, HandoffBehavior.SnapshotAndReplace, true);
                    storyboard.SetSpeedRatio(ellipse, spinner.RotationSpeed);
                }
                else
                    storyboard.Remove(ellipse);
            }
        }

        public static readonly DependencyProperty DiameterProperty =
            DependencyProperty.Register(nameof(Diameter), typeof(double), typeof(LoadingSpinner), new PropertyMetadata(50.0));

        public static readonly DependencyProperty ThicknessProperty =
            DependencyProperty.Register(nameof(Thickness), typeof(double), typeof(LoadingSpinner), new PropertyMetadata(1.0));

        public static readonly DependencyProperty LineRatioProperty =
            DependencyProperty.Register(nameof(LineRatio), typeof(double), typeof(LoadingSpinner), new PropertyMetadata(0.75));

        public static readonly DependencyProperty CapProperty =
            DependencyProperty.Register(nameof(Cap), typeof(PenLineCap), typeof(LoadingSpinner), new PropertyMetadata(PenLineCap.Flat));

        public static readonly DependencyProperty RotationSpeedProperty =
            DependencyProperty.Register(nameof(RotationSpeed), typeof(double), typeof(LoadingSpinner), new PropertyMetadata(1.0));

        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }

        public double Diameter
        {
            get { return (double)GetValue(DiameterProperty); }
            set { SetValue(DiameterProperty, value); }
        }

        public double Thickness
        {
            get { return (double)GetValue(ThicknessProperty); }
            set { SetValue(ThicknessProperty, value); }
        }

        public double LineRatio
        {
            get { return (double)GetValue(LineRatioProperty); }
            set { SetValue(LineRatioProperty, value); }
        }

        public PenLineCap Cap
        {
            get { return (PenLineCap)GetValue(CapProperty); }
            set { SetValue(CapProperty, value); }
        }

        public double RotationSpeed
        {
            get { return (double)GetValue(RotationSpeedProperty); }
            set { SetValue(RotationSpeedProperty, value); }
        }

        static LoadingSpinner()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LoadingSpinner), new FrameworkPropertyMetadata(typeof(LoadingSpinner)));
        }
    }
}
