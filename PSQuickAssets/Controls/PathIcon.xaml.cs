using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace PSQuickAssets.Controls;

public partial class PathIcon : UserControl
{
    public static readonly DependencyProperty DataProperty =
        DependencyProperty.Register(nameof(Data), typeof(Geometry), typeof(PathIcon), new PropertyMetadata(null));

    public static readonly DependencyProperty FillProperty =
        DependencyProperty.Register(nameof(Fill), typeof(SolidColorBrush), typeof(PathIcon), new PropertyMetadata(Brushes.Black, OnBrushChanged));

    public static readonly DependencyProperty FillMouseOverProperty =
        DependencyProperty.Register(nameof(FillMouseOver), typeof(SolidColorBrush), typeof(PathIcon), new PropertyMetadata(null));

    public static readonly DependencyProperty CommandProperty =
        DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(PathIcon), new PropertyMetadata(null));

    public static readonly DependencyProperty CommandParameterProperty =
        DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(PathIcon), new PropertyMetadata(null));

    public Geometry Data
    {
        get { return (Geometry)GetValue(DataProperty); }
        set { SetValue(DataProperty, value); }
    }

    public SolidColorBrush Fill
    {
        get { return (SolidColorBrush)GetValue(FillProperty); }
        set { SetValue(FillProperty, value); }
    }

    public SolidColorBrush FillMouseOver
    {
        get { return (SolidColorBrush)GetValue(FillMouseOverProperty); }
        set { SetValue(FillMouseOverProperty, value); }
    }

    public ICommand Command
    {
        get { return (ICommand)GetValue(CommandProperty); }
        set { SetValue(CommandProperty, value); }
    }

    public object CommandParameter
    {
        get { return GetValue(CommandProperty); }
        set { SetValue(CommandProperty, value); }
    }

    private SolidColorBrush _brush;

    public PathIcon()
    {
        InitializeComponent();

        _brush = (SolidColorBrush)FillProperty.DefaultMetadata.DefaultValue;
        box.MouseEnter += Box_MouseEnter;
        box.MouseLeave += Box_MouseLeave;
        box.MouseLeftButtonDown += Box_MouseLeftButtonDown;
    }

    private static void OnBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        PathIcon path = (PathIcon)d;
        path._brush = new SolidColorBrush(((SolidColorBrush)e.NewValue).Color);
    }

    private void Box_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (Command is not null)
        {
            e.Handled = true;
            Command.Execute(CommandParameter);
        }
    }

    private void Box_MouseEnter(object sender, MouseEventArgs e)
    {
        if (FillMouseOver is null) return;
        AnimateFill(FillMouseOver.Color, TimeSpan.FromMilliseconds(30));
    }

    private void Box_MouseLeave(object sender, MouseEventArgs e)
    {
        if (Fill is null) return;
        AnimateFill(Fill.Color, TimeSpan.FromMilliseconds(150));
    }

    private void AnimateFill(Color toValue, TimeSpan duration)
    {
        path.Fill = _brush;
        ColorAnimation colorAnimation = new ColorAnimation(toValue, new Duration(duration));
        _brush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);
    }
}