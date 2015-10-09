using System;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.LegacyAttributeSupport
{
    /// <summary>
    /// Used to implicitly mark a class as a Plugin candidate for StructureMap
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PluggableAttribute : StructureMapAttribute
    {
        public PluggableAttribute(string concreteKey)
        {
            ConcreteKey = concreteKey;
        }


        /// <summary>
        /// The ConcreteKey alias of the Type
        /// </summary>
        public string ConcreteKey { get; set; }

        /// <summary>
        /// Determines whether a Type object is marked as Pluggable
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public static bool MarkedAsPluggable(Type objectType)
        {
            var att = GetCustomAttribute(objectType, typeof (PluggableAttribute), false) as PluggableAttribute;
            return (att != null);
        }

        public override void Alter(IConfiguredInstance instance)
        {
  
        }
    }
}