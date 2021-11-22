using MLogger;
using System;
using System.Reflection;
using System.Text.Json.Serialization;

namespace PSQuickAssets.Configuration;

public abstract class BaseConfig
{
    public event Action<string>? ConfigPropertyChanged;

    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public bool SavesOnPropertyChanged { get; }

    protected IConfigHandler _configHandler;
    private readonly ILogger? _logger;

    public BaseConfig(IConfigHandler configHandler, ILogger? logger, bool shouldSaveOnPropertyChanged)
    {
        _configHandler = configHandler;
        _logger = logger;

        SavesOnPropertyChanged = shouldSaveOnPropertyChanged;
    }

    /// <summary>
    /// Sets the value of specified property and rises <see cref="ConfigPropertyChanged"/> event with name of this property.
    /// </summary>
    /// <param name="propertyName">Property that will be set.</param>
    /// <param name="value">New value for this property.</param>
    /// <exception cref="ArgumentNullException">If propertyName is null or white space. Or value is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If Property is not found.</exception>
    /// <exception cref="Exception">If value does not match the type of Property.</exception>
    public virtual void SetValue(string propertyName, object value)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            _logger?.Error($"[Config] - propertyName cannot be null or empty.");
            throw new ArgumentNullException(nameof(propertyName), "Property Name cannot be null or empty");
        }
        else if (value is null)
        {
            _logger?.Error($"[Config] - value cannot be null.");
            throw new ArgumentNullException(nameof(value), "Value cannot be null");
        }

        PropertyInfo? property = (typeof(Config)).GetProperty(propertyName);

        if (property is null)
        {
            _logger?.Error($"[Config] - Failed to set config value of <{propertyName}>: No such property exists. GetProperty({propertyName}) returned null.");
            throw new ArgumentOutOfRangeException(propertyName, $"Property <{propertyName}> not found on type '{nameof(Config)}'");
        }

        Type propertyType = property.PropertyType;
        Type newValueType = value.GetType();

        if (propertyType != newValueType)
        {
            _logger?.Error($"[Config] - Failed to set config value of <{propertyName}>: Types don't match. Property type: <{propertyType}>, passed Value type: <{newValueType}>.");
            throw new Exception("Cannot set config value of <{propertyName}>: Types don't match. Property type: <{propertyType}>, passed Value type: <{newValueType}>.");
        }

        object? prevValue = property.GetValue(this);

        if (value.Equals(prevValue))
        {
            _logger?.Info($"[Config] - New value for <{propertyName}> has not been set: Values is already equal.");
            return;
        }

        property.SetValue(this, value);
        ConfigPropertyChanged?.Invoke(propertyName);
        _logger?.Info($"[Config] - {propertyName} is set to <{value}>");

        if (SavesOnPropertyChanged)
            Save();
    }

    /// <summary>
    /// Safely tries to set value of a specified Property.
    /// </summary>
    /// <param name="propertyName">Property that will be set.</param>
    /// <param name="value">New value for this property.</param>
    /// <returns>Error message if not successful.</returns>
    public virtual string TrySetValue(string propertyName, object value)
    {
        try
        {
            SetValue(propertyName, value);
            return string.Empty;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public T Load<T>() where T : BaseConfig
    {
        var jsonElements = _configHandler.Load();

        if (jsonElements.Count == 0)
        {
            _logger?.Warn("[Config - Loading] - No properties have been loaded from json. Default config values would be loaded.");
        }
        else
        {
            var properties = (typeof(Config)).GetProperties();

            foreach (var prop in properties)
            {
                try
                {
                    prop.SetValue(this, jsonElements[prop.Name]);
                }
                catch (Exception ex)
                {
                    _logger?.Error($"[Config - Loading] - Failed to set value of <{prop.Name}>:\n{ex}");
                }
            }
        }

        return (T)this;
    }

    public void Save()
    {
        _configHandler.Save(this);
    }
}