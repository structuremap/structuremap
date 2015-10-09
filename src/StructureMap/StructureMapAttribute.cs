using System;
using System.Reflection;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap
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


    /// <summary>
    /// Makes StructureMap treat a Type as a singleton in the lifecycle scoping
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class SingletonAttribute : StructureMapAttribute
    {
        public override void Alter(PluginFamily family)
        {
            family.SetLifecycleTo<SingletonLifecycle>();
        }

        public override void Alter(IConfiguredInstance instance)
        {
            instance.SetLifecycleTo<SingletonLifecycle>();
        }
    }

    /// <summary>
    /// Makes StructureMap treat a Type with the AlwaysUnique lifecycle
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class AlwaysUniqueAttribute : StructureMapAttribute
    {
        public override void Alter(PluginFamily family)
        {
            family.SetLifecycleTo<UniquePerRequestLifecycle>();
        }

        public override void Alter(IConfiguredInstance instance)
        {
            instance.SetLifecycleTo<UniquePerRequestLifecycle>();
        }
    }

}