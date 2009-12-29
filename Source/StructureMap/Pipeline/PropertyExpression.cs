using System;
using System.Configuration;

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

        public PropertyExpression(ConstructorInstance instance, string propertyName)
        {
            _instance = instance;
            _propertyName = propertyName;
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
            string propertyValue = ConfigurationManager.AppSettings[appSettingKey];
            if (propertyValue == null) propertyValue = defaultValue;
            _instance.SetValue(_propertyName, propertyValue);
            return (T) _instance;
        }
    }
}