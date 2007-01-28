using System;

namespace StructureMap
{
    /// <summary>
    /// Used to implicitly mark a class as a Plugin candidate for StructureMap
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PluggableAttribute : Attribute
    {
        private string _concreteKey;
        private string _description;

        public PluggableAttribute(string concreteKey)
            : this(concreteKey, string.Empty)
        {
        }

        public PluggableAttribute(string TypeName, string Description)
        {
            _concreteKey = TypeName;
            _description = Description;
        }

        /// <summary>
        /// The ConcreteKey alias of the Type
        /// </summary>
        public string ConcreteKey
        {
            get { return _concreteKey; }
            set { _concreteKey = value; }
        }

        /// <summary>
        /// Description of the pluggable class type
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        /// Gets an instance of PluggableAttribute from a Type object 
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public static PluggableAttribute InstanceOf(Type objectType)
        {
            return (PluggableAttribute) GetCustomAttribute(objectType, typeof (PluggableAttribute), false);
        }

        /// <summary>
        /// Determines whether a Type object is marked as Pluggable
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public static bool MarkedAsPluggable(Type objectType)
        {
            PluggableAttribute att = InstanceOf(objectType);
            return (att != null);
        }
    }
}