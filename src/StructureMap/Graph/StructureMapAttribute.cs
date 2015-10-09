using System;
using System.Reflection;
using StructureMap.Pipeline;

namespace StructureMap.Graph
{
    /// <summary>
    /// Base 
    /// </summary>
    public abstract class StructureMapAttribute : Attribute
    {
        /// <summary>
        /// Override this method to apply a configuration change to an entire
        /// PluginFamily (every Instance of a certain PluginType)
        /// </summary>
        /// <param name="family"></param>
        public virtual void Alter(PluginFamily family)
        {
            // Nothing
        }

        /// <summary>
        /// Make configuration alterations to a single IConfiguredInstance object
        /// </summary>
        /// <param name="instance"></param>
        public virtual void Alter(IConfiguredInstance instance)
        {
            // Nothing
        }

        /// <summary>
        /// Apply configuration customization to a single setter property
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="property"></param>
        public virtual void Alter(IConfiguredInstance instance, PropertyInfo property)
        {
            
        }

        /// <summary>
        /// Apply configuration customization for a single constructor parameter
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="parameter"></param>
        public virtual void Alter(IConfiguredInstance instance, ParameterInfo parameter)
        {
            
        }
    }

}