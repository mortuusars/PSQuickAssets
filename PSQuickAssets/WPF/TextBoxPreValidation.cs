using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace PSQuickAssets.WPF;

/// <summary>
/// Allows the validation of the Text to be performed without setting source property.<br></br>
/// Uses a substitute binding with validation to do it.<br></br>
/// Original binding then updated when control loses focus.
/// </summary>
public class TextBoxPreValidation
{
    public static readonly DependencyProperty ValidationDelegateProperty =
    DependencyProperty.RegisterAttached("ValidationDelegate", typeof(Func<string, IEnumerable<string>>),
        typeof(TextBoxPreValidation), new PropertyMetadata(new Func<string,
            IEnumerable<string>>((_) => Array.Empty<string>()), OnValidationDelegateChanged));
    public static Func<string, IEnumerable<string>> GetValidationDelegate(DependencyObject obj) => (Func<string, IEnumerable<string>>)obj.GetValue(ValidationDelegateProperty);
    public static void SetValidationDelegate(DependencyObject obj, Func<string, IEnumerable<string>> value) => obj.SetValue(ValidationDelegateProperty, value);

    private static readonly Dictionary<TextBox, (TextBoxAttachedErrorValidation validation, Binding originalBinding)> _textBoxState = new();

    private static void OnValidationDelegateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not TextBox textBox)
            return;

        textBox.GotFocus -= OnGotFocus;
        textBox.LostFocus -= OnLostFocus;

        textBox.LostFocus += OnLostFocus;
        textBox.GotFocus += OnGotFocus;
    }

    /// <summary>
    /// Changes Text binding of a TextBox to the own binding with validation.
    /// Oirignal binding is stored in a dictionary to be restored later.
    /// </summary>
    private static void OnGotFocus(object sender, RoutedEventArgs e)
    {
        if (sender is not TextBox textBox)
            return;

        _textBoxState.Remove(textBox);

        var errorValidation = new TextBoxAttachedErrorValidation(textBox.Text, GetValidationDelegate(textBox));
        var textBinding = BindingOperations.GetBinding(textBox, TextBox.TextProperty);

        _textBoxState.Add(textBox, (errorValidation, textBinding));

        // Create our custom binding with validation:
        Binding validationBinding = new();
        validationBinding.Source = errorValidation;
        validationBinding.Path = new PropertyPath("Text");
        validationBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
        validationBinding.ValidatesOnNotifyDataErrors = true;

        textBox.SetBinding(TextBox.TextProperty, validationBinding);
    }

    /// <summary>
    /// Restores original Text binding and updates its source with a new value.
    /// Original value will be used if Binding had validation errors.
    /// </summary>
    private static void OnLostFocus(object sender, RoutedEventArgs e)
    {
        if (sender is not TextBox textBox)
            return;

        if (!_textBoxState.TryGetValue(textBox, out var state))
            return;

        // Restore original binding:
        textBox.SetBinding(TextBox.TextProperty, state.originalBinding);

        if (state.validation.HasErrors)
            textBox.Text = state.validation.InitialText; // Reset to previous state if validation failed.
        else
            textBox.Text = state.validation.Text; // Validation succeded - set new value.

        // Update original source with new value:
        BindingOperations.GetBindingExpression(textBox, TextBox.TextProperty).UpdateSource();
    }
}

/// <summary>
/// Custom "ViewModel" to allow validation without setting source property.
/// </summary>
public class TextBoxAttachedErrorValidation : ObservableObject, INotifyDataErrorInfo
{
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    /// <summary>
    /// Gets or sets current text. Validated on PropertyChanged.
    /// </summary>
    public string Text
    {
        get => _text;
        set
        {
            _text = value;
            CheckForErrors(Text);
            OnPropertyChanged(nameof(Text));
        }
    }

    /// <summary>
    /// Original value before the operation.
    /// </summary>
    public string InitialText { get; }

    public bool HasErrors => _errors.Count > 0;

    private readonly List<string> _errors = new();

    private string _text;
    private readonly Func<string, IEnumerable<string>> _validationDelegate;

    public TextBoxAttachedErrorValidation(string initialText, Func<string, IEnumerable<string>> validationDelegate)
    {
        InitialText = initialText;
        _text = initialText;
        _validationDelegate = validationDelegate;
    }

    public IEnumerable GetErrors(string? propertyName) => _errors;

    private void CheckForErrors(string text)
    {
        _errors.Clear();

        // Only validate when text is different from the original.
        if (text != InitialText)
            _errors.AddRange(_validationDelegate.Invoke(text));

        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(Text)));
    }
}
