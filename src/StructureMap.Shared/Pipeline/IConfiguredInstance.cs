using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StructureMap.Building.Interception;
using StructureMap.TypeRules;

namespace StructureMap.Pipeline
{
    // SAMPLE: IConfiguredInstance
    /// <summary>
    /// Represents a configured Instance object that
    /// is built by StructureMap directly by calling 
    /// constructor functions and property setters
    /// </summary>
    public interface IConfiguredInstance
    {
        /// <summary>
        /// The Instance name
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The actual concrete type built by this Instance
        /// </summary>
        Type PluggedType { get; }

        /// <summary>
        /// The explicitly configured inline dependencies that override
        /// auto-wiring
        /// </summary>
        DependencyCollection Dependencies { get; }

        /// <summary>
        /// Add an interceptor to only this Instance
        /// </summary>
        /// <param name="interceptor"></param>
        void AddInterceptor(IInterceptor interceptor);

        /// <summary>
        /// Set the lifecycle of only this Instance to the ILifecycle type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <example>
        /// SetLifecycleTo<SingletonLifecycle>()
        /// </example>
        void SetLifecycleTo<T>() where T : ILifecycle, new();

        /// <summary>
        /// Set the lifecycle of only this Instance to a certain lifecycle
        /// </summary>
        /// <param name="lifecycle"></param>
        void SetLifecycleTo(ILifecycle lifecycle);

        /// <summary>
        /// The current Lifecycle that will be used for this Instance
        /// </summary>
        ILifecycle Lifecycle { get; }

        /// <summary>
        /// Explicitly choose a constructor
        /// </summary>
        ConstructorInfo Constructor { get; set; }

        /// <summary>
        /// Has a build plan already been created for this instance?
        /// </summary>
        /// <returns></returns>
        bool HasBuildPlan();

        /// <summary>
        /// Clears out any cached IBuildPlan for this Instance.
        /// </summary>
        void ClearBuildPlan();
    }
    // ENDSAMPLE


    public static class ConfiguredInstanceExtensions
    {
        /// <summary>
        /// Set the lifecycle of this instance to "singleton"
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static IConfiguredInstance Singleton(this IConfiguredInstance instance)
        {
            instance.SetLifecycleTo<SingletonLifecycle>();

            return instance;
        }

        /// <summary>
        /// Set the lifecycle of this instance to the default "transient" lifecycle
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static IConfiguredInstance DefaultLifecycle(this IConfiguredInstance instance)
        {
            instance.SetLifecycleTo<TransientLifecycle>();
            return instance;
        }

        /// <summary>
        /// Gets an enumerable of all the public, settable properties that could be used
        /// for setter injection by StructureMap
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> SettableProperties(this IConfiguredInstance instance)
        {
            return instance.PluggedType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.CanWrite && x.GetSetMethod(false) != null && x.GetSetMethod().GetParameters().Length == 1);
        }
    }
}