using MLogger;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json.Serialization;

namespace PSQuickAssets.Configuration;

public abstract class ConfigBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public bool SaveOnPropertyChanged { get; }

    protected IConfigSaver _configSaver;
    private readonly ILogger? _logger;

    public ConfigBase(IConfigSaver configHandler, ILogger? logger, bool shouldSaveOnPropertyChanged)
    {
        _configSaver = configHandler;
        _logger = logger;

        SaveOnPropertyChanged = shouldSaveOnPropertyChanged;
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
            _logger?.Info($"[Config] - New value for <{propertyName}> has not been set: Values already equal.");
            return;
        }

        property.SetValue(this, value);
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        _logger?.Debug($"[Config] - {propertyName} is set to <{value}>");

        if (SaveOnPropertyChanged)
            Save();
    }

    /// <summary>
    /// Safely tries to set value of a specified Property.
    /// </summary>
    /// <param name="propertyName">Property that will be set.</param>
    /// <param name="value">New value for this property.</param>
    /// <returns>Error message if not successful.</returns>
    public virtual bool TrySetValue(string propertyName, object value, out string message)
    {
        try
        {
            SetValue(propertyName, value);
            message = string.Empty;
            return true;
        }
        catch (Exception ex)
        {
            message = ex.Message;
            return false;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T Load<T>(IConfigLoader loader) where T : ConfigBase
    {
        Dictionary<string, object> loadedProperties = loader.Load();

        if (loadedProperties.Count == 0)
        {
            _logger?.Warn("[Config - Loading] - No properties have been loaded. Default config values would be used.");
            return (T)this;
        }

        var properties = (typeof(Config)).GetProperties();
        bool hasMissingSettings = false;

        foreach (var prop in properties)
        {
            //Skip property in the base class. This property is configured when config is instantiated.
            if (prop.Name.Equals(nameof(SaveOnPropertyChanged)))
                continue;

            try
            {
                prop.SetValue(this, loadedProperties[prop.Name]);
            }
            catch (KeyNotFoundException)
            {
                _logger?.Error($"[Config - Loading] - Failed to set value of <{prop.Name}>: This setting was probably missing from config file.");
                hasMissingSettings = true;
            }
            catch (Exception ex)
            {
                _logger?.Error($"[Config - Loading] - Failed to set value of <{prop.Name}>:\n{ex}");
                hasMissingSettings = true;
            }
        }

        if (hasMissingSettings)
        {
            _logger?.Debug($"[Config - Loading] - Saving config because missing settings were found...");
            Save();
        }

        return (T)this;
    }

    /// <summary>
    /// Saves the config through the provided IConfigHandler.
    /// </summary>
    public void Save()
    {
        _configSaver.Save(this);
    }
}