using System.Configuration;

namespace StructureMap.Configuration.DSL
{
    /// <summary>
    /// Defines the value of a primitive argument to a constructur argument
    /// </summary>
    public class PropertyExpression
    {
        private readonly InstanceExpression _instance;
        private readonly MemoryInstanceMemento _memento;
        private readonly string _propertyName;

        public PropertyExpression(InstanceExpression instance, MemoryInstanceMemento memento, string propertyName)
        {
            _instance = instance;
            _memento = memento;
            _propertyName = propertyName;
        }

        /// <summary>
        /// Sets the value of the constructor argument
        /// </summary>
        /// <param name="propertyValue"></param>
        /// <returns></returns>
        public InstanceExpression EqualTo(object propertyValue)
        {
            _memento.SetProperty(_propertyName, propertyValue.ToString());
            return _instance;
        }

        /// <summary>
        /// Sets the value of the constructor argument to the key/value in the 
        /// AppSettings
        /// </summary>
        /// <param name="appSettingKey"></param>
        /// <returns></returns>
        public InstanceExpression EqualToAppSetting(string appSettingKey)
        {
            string propertyValue = ConfigurationManager.AppSettings[appSettingKey];
            _memento.SetProperty(_propertyName, propertyValue);
            return _instance;
        }
    }
}