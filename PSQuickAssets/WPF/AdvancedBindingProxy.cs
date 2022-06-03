using System.Windows;

namespace PSQuickAssets.WPF;

public class AdvancedBindingProxy : Freezable
{
    public Type DataType
    {
        get { return (Type)GetValue(DataTypeProperty); }
        set { SetValue(DataTypeProperty, value); }
    }

    public static readonly DependencyProperty DataTypeProperty =
        DependencyProperty.Register(nameof(DataType), typeof(Type), typeof(AdvancedBindingProxy), new PropertyMetadata(null));

    public object Data
    {
        get { 
            return DataType is not null ? 
                Convert.ChangeType(GetValue(DataProperty), DataType) :
                (object)GetValue(DataProperty); }
        set { SetValue(DataProperty, value); }
    }

    public static readonly DependencyProperty DataProperty =
        DependencyProperty.Register(nameof(Data), typeof(object), typeof(AdvancedBindingProxy), new PropertyMetadata(null));


    protected override Freezable CreateInstanceCore()
    {
        return new AdvancedBindingProxy();
    }
}
