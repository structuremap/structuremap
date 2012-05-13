using System;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Reflection;

namespace StructureMap.Pipeline
{
    /// <summary>
    /// Defines the value of a primitive argument to a constructur argument
    /// </summary>
    [Obsolete("Change to DependencyExpression<CTORTYPE> instead")]
    public class PropertyExpression<T> where T : ConstructorInstance
    {
        private readonly ConstructorInstance _instance;
        private readonly string _propertyName;
        private readonly TypeConverter[] _converters;
        private readonly PropertyInfo _propertyInfo;

        public PropertyExpression(ConstructorInstance instance, string propertyName)
        {
            _instance = instance;
            _propertyName = propertyName;
            _propertyInfo = instance.PluggedType.GetProperty(propertyName);
            _converters = _propertyInfo.GetCustomAttributes(typeof (TypeConverterAttribute), true)
                .OfType<TypeConverterAttribute>()
                .Select(attr => Type.GetType(attr.ConverterTypeName))
                .Select(type => (TypeConverter) Activator.CreateInstance(type))
                .ToArray();
        }

        /// <summary>
        /// Sets the value of the constructor argument
        /// </summary>
        /// <param name="propertyValue"></param>
        /// <returns></returns>
        public T EqualTo(object propertyValue)
        {
            _instance.SetValue(_propertyName, propertyValue);

            return (T) _instance;
        }

        /// <summary>
        /// Sets the value of the constructor argument to the key/value in the 
        /// AppSettings
        /// </summary>
        /// <param name="appSettingKey">The key in appSettings for the value to use.</param>
        /// <returns></returns>
        public T EqualToAppSetting(string appSettingKey)
        {
            return EqualToAppSetting(appSettingKey, null);
        }

        /// <summary>
        /// Sets the value of the constructor argument to the key/value in the 
        /// AppSettings when it exists. Otherwise uses the provided default value.
        /// </summary>
        /// <param name="appSettingKey">The key in appSettings for the value to use.</param>
        /// <param name="defaultValue">The value to use if an entry for <paramref name="appSettingKey"/> does not exist in the appSettings section.</param>
        /// <returns></returns>
        [Obsolete("Change to using a func to get this")]
        public T EqualToAppSetting(string appSettingKey, string defaultValue)
        {
            return EqualToAppSetting(appSettingKey, defaultValue, null);
        }

        /// <summary>
        /// Sets the value of the constructor argument to the key/value in the 
        /// AppSettings when it exists. Otherwise uses the provided default value.
        /// The value is converted using the provided TypeConverter.
        /// </summary>
        /// <param name="appSettingKey">The key in appSettings for the value to use.</param>
        /// <param name="defaultValue">The value to use if an entry for <paramref name="appSettingKey"/> does not exist in the appSettings section.</param>
        /// <param name="converter">The TypeConverter to use for converting non-string values.</param>
        /// <returns></returns>
        [Obsolete("Change to using a func to get this")]
        public T EqualToAppSetting(string appSettingKey, string defaultValue, TypeConverter converter)
        {
            object propertyValue = ConfigurationManager.AppSettings[appSettingKey] ?? defaultValue;
            converter = converter
                ?? _converters.FirstOrDefault(c => c.CanConvertTo(_propertyInfo.PropertyType) && c.CanConvertFrom(typeof (string)))
                ?? TypeDescriptor.GetConverter(_propertyInfo.PropertyType);
            if (converter != null) propertyValue = converter.ConvertFrom(propertyValue);
            _instance.SetValue(_propertyName, propertyValue);
            return (T) _instance;
        }
    }
}