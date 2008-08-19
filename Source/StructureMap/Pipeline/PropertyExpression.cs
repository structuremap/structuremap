using System.Configuration;

namespace StructureMap.Pipeline
{
    /// <summary>
    /// Defines the value of a primitive argument to a constructur argument
    /// </summary>
    public class PropertyExpression<T> where T : IConfiguredInstance
    {
        private readonly IConfiguredInstance _instance;
        private readonly string _propertyName;

        public PropertyExpression(IConfiguredInstance instance, string propertyName)
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
            _instance.SetProperty(_propertyName, propertyValue.ToString());
            return (T) _instance;
        }

        /// <summary>
        /// Sets the value of the constructor argument to the key/value in the 
        /// AppSettings
        /// </summary>
        /// <param name="appSettingKey"></param>
        /// <returns></returns>
        public T EqualToAppSetting(string appSettingKey)
        {
            string propertyValue = ConfigurationManager.AppSettings[appSettingKey];
            _instance.SetProperty(_propertyName, propertyValue);
            return (T) _instance;
        }
    }
}