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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PSQuickAssets.Controls;

public partial class ToggleSwitchControl : UserControl
{
    public static readonly DependencyProperty CornerRadiusProperty =
        DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(ToggleSwitchControl), new PropertyMetadata(new CornerRadius(0)));

    public CornerRadius CornerRadius
    {
        get { return (CornerRadius)GetValue(CornerRadiusProperty); }
        set { SetValue(CornerRadiusProperty, value); }
    }




    public ToggleSwitchControl()
    {
        InitializeComponent();

        checkBox.Checked += CheckBox_Checked;
    }

    private void CheckBox_Checked(object sender, RoutedEventArgs e)
    {
        var ellipse = (Ellipse)checkBox.Template.FindName("Toggle", checkBox);

        //ThicknessAnimation anim = new ThicknessAnimation(new Thickness());
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        
        var border = (Border)checkBox.Template.FindName("Border", checkBox);
        border.CornerRadius = new CornerRadius(this.ActualWidth / 2);



    }
}