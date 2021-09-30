using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LoadingSpinnerControl
{
    public class LoadingSpinner : Control
    {
        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register(nameof(IsLoading), typeof(bool), typeof(LoadingSpinner), new PropertyMetadata(false));

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
