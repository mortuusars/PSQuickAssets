using System;
using System.Windows.Input;
using System.Windows.Markup;

namespace PSQuickAssets.WPF;

public enum MouseWheelDirection { Up, Down }

public class MouseWheelGesture : MouseGesture
{
    public MouseWheelDirection Direction { get; set; }

    public MouseWheelGesture(ModifierKeys modifiers, MouseWheelDirection direction) : base(MouseAction.WheelClick, modifiers)
    {
        Direction = direction;
    }

    public override bool Matches(object targetElement, InputEventArgs inputEventArgs)
    {
        if (inputEventArgs is KeyEventArgs)
            return false;

        if (inputEventArgs is not MouseWheelEventArgs args)
            return false;

        //if (!base.Matches(targetElement, inputEventArgs))
            //return false;

        if (Direction == MouseWheelDirection.Up && args.Delta > 0 || Direction == MouseWheelDirection.Down && args.Delta < 0)
        {
            inputEventArgs.Handled = true;
            return true;
        }

        return false;
    }    
}

public class MouseWheel : MarkupExtension
{
    public MouseWheelDirection Direction { get; set; }
    public ModifierKeys Keys { get; set; }

    public MouseWheel()
    {
        Direction = MouseWheelDirection.Down;
        Keys = ModifierKeys.None;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return new MouseWheelGesture(Keys, Direction);
    }
}