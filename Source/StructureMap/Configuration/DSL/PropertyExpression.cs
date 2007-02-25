using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace StructureMap.Configuration.DSL
{
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

        public InstanceExpression EqualTo(object propertyValue)
        {
            _memento.SetProperty(_propertyName, propertyValue.ToString());
            return _instance;
        }

        public InstanceExpression EqualToAppSetting(string appSettingKey)
        {
            string propertyValue = ConfigurationManager.AppSettings[appSettingKey];
            _memento.SetProperty(_propertyName, propertyValue);
            return _instance;
        }
    }
}
