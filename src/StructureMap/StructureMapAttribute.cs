using System;
using System.Reflection;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap
{
    // SAMPLE: StructureMapAttribute
    /// <summary>
    /// Base class for custom configuration attributes
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
        }

        /// <summary>
        /// Make configuration alterations to a single IConfiguredInstance object
        /// </summary>
        /// <param name="instance"></param>
        public virtual void Alter(IConfiguredInstance instance)
        {
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
    // ENDSAMPLE
}