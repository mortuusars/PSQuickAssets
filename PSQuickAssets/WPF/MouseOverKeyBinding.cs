using MortuusUI.Extensions;
using System;
using System.Media;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace PSQuickAssets.WPF;

internal enum PassedObjectType
{
    DataContext,
    Element
}

/// <summary>
/// Wrapper command that performs a HitTest when executed. Passes result of a HitTest to wrapped command as a parameter.
/// </summary>
internal class HitTestWrapperCommand : ICommand
{
    public event EventHandler? CanExecuteChanged { add { CommandManager.RequerySuggested += value; } remove { CommandManager.RequerySuggested -= value; } }

    /// <summary>
    /// Wrapped command.
    /// </summary>
    public ICommand? OriginalCommand { get; set; }
    /// <summary>
    /// Defines how the element will be passed to the command.
    /// </summary>
    public PassedObjectType PassedObjectType { get; set; } = PassedObjectType.DataContext;
    /// <summary>
    /// Name of the <see cref="FrameworkElement"/> to find.<br></br>This takes precedence over <see cref="ElementType"/>.
    /// </summary>
    public string ElementName { get; set; } = string.Empty;
    /// <summary>
    /// Type of the element to find.
    /// </summary>
    public Type ElementType { get; set; } = typeof(FrameworkElement);

    public bool CanExecute(object? parameter) => true;

    public void Execute(object? parameter)
    {
        if (OriginalCommand is null) // Nothing to execute.
            return;

        // DependencyObject can in theory be passed too (and we then get a Window from it), but Window will do for now.
        if (parameter is not Window window) // Window is required to do hit test relative to it. 
            throw new ArgumentNullException(nameof(parameter), "Object of type Window should passed as CommandParameter.");

        var mousePos = Mouse.GetPosition(window);
        var hitTestResult = VisualTreeHelper.HitTest(window, mousePos);

        if (hitTestResult.VisualHit is Visual hitTestVisual && GetMatchedElement(hitTestVisual) is Visual matchedElement)
        {
            if (PassedObjectType == PassedObjectType.Element)
            {
                OriginalCommand.Execute(matchedElement);
                return;
            }

            if (PassedObjectType == PassedObjectType.DataContext && matchedElement is FrameworkElement frameworkElement)
            {
                OriginalCommand.Execute(frameworkElement.DataContext);
                return;
            }
        }

        // Error:
        SystemSounds.Asterisk.Play();
    }

    private Visual? GetMatchedElement(Visual hitTestVisual)
    {
        if (string.IsNullOrWhiteSpace(ElementName))
            return MatchByType(hitTestVisual);

        return hitTestVisual.FindAncestorByName(ElementName) as Visual;
    }

    private Visual? MatchByType(Visual hitTestVisual)
    {
        if (hitTestVisual.GetType() == ElementType)
            return hitTestVisual;

        return hitTestVisual.GetParentOfType(ElementType) as Visual;
    }
}

/// <summary>
/// Defines a <see cref="KeyBinding"/> that passes UI element to a command as a parameter.
/// </summary>
internal class MouseOverKeyBinding : KeyBinding
{
    static MouseOverKeyBinding()
    {
        CommandProperty.OverrideMetadata(typeof(MouseOverKeyBinding), new FrameworkPropertyMetadata(null, (_, _) => { }, CommandChanging));
    }

    /// <summary>
    /// Name of the <see cref="FrameworkElement"/> to find.<br></br>This takes precedence over <see cref="ElementType"/>.
    /// </summary>
    public string ElementName
    {
        get { return (string)GetValue(ElementNameProperty); }
        set { SetValue(ElementNameProperty, value); }
    }

    public static readonly DependencyProperty ElementNameProperty =
        DependencyProperty.Register(nameof(ElementName), typeof(string), typeof(MouseOverKeyBinding), new PropertyMetadata(string.Empty));

    /// <summary>
    /// Type of the element to find.
    /// </summary>
    public Type ElementType
    {
        get { return (Type)GetValue(ElementTypeProperty); }
        set { SetValue(ElementTypeProperty, value); }
    }

    public static readonly DependencyProperty ElementTypeProperty =
        DependencyProperty.Register(nameof(ElementType), typeof(Type), typeof(MouseOverKeyBinding), new PropertyMetadata(typeof(FrameworkElement)));

    /// <summary>
    /// Defines how the element will be passed to the command.
    /// </summary>
    public PassedObjectType PassedObjectType
    {
        get { return (PassedObjectType)GetValue(PassedObjectTypeProperty); }
        set { SetValue(PassedObjectTypeProperty, value); }
    }

    public static readonly DependencyProperty PassedObjectTypeProperty =
        DependencyProperty.Register(nameof(PassedObjectType), typeof(PassedObjectType), typeof(MouseOverKeyBinding), new PropertyMetadata(PassedObjectType.DataContext));

    // Intercepts assigned command and wraps it with HitTestWrapper:
    private static object CommandChanging(DependencyObject d, object baseValue)
    {
        MouseOverKeyBinding KeyBinding = (MouseOverKeyBinding)d;
        if (baseValue is not ICommand newCommand)
            return baseValue;

        return new HitTestWrapperCommand
        {
            OriginalCommand = newCommand,
            PassedObjectType = KeyBinding.PassedObjectType,
            ElementName = KeyBinding.ElementName,
            ElementType = KeyBinding.ElementType,
        };
    }
}
