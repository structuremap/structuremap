using System;
using System.Collections.Generic;

namespace StructureMap.Pipeline
{
    public interface IConfiguredInstance
    {
        InstanceMemento[] GetChildrenArray(string propertyName);
        string GetProperty(string propertyName);
        object GetChild(string propertyName, string typeName, IInstanceCreator instanceCreator);
    }

    public class ConfiguredInstance : Instance
    {
        private readonly Dictionary<string, string> _properties = new Dictionary<string, string>();
        private Dictionary<string, Instance> _children = new Dictionary<string, Instance>();

        protected override object build(Type type, IInstanceCreator creator)
        {
            throw new NotImplementedException();
        }

        public string GetProperty(string propertyName)
        {
            if (!_properties.ContainsKey(propertyName))
            {
                throw new StructureMapException(205, propertyName, Name);
            }

            return _properties[propertyName];
        }

        public ConfiguredInstance SetProperty(string propertyName, string propertyValue)
        {
            _properties[propertyName] = propertyValue;
            return this;
        }
    }
}