using System.Reflection;

namespace StructureMap.Graph
{
    /// <summary>
    /// Represents a PropertyInfo of a Plugin.PluggedType that is filled by Setter Injection
    /// </summary>
    public class SetterProperty
    {
        private readonly PropertyInfo _property;

        public SetterProperty(PropertyInfo property) : base()
        {
            _property = property;
        }

        public PropertyInfo Property
        {
            get { return _property; }
        }

        public string Name
        {
            get { return _property.Name; }
        }
    }
}