using System;
using System.Reflection;
using StructureMap.Building.Interception;

namespace StructureMap.Pipeline
{
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
    }

    internal interface IOverridableInstance
    {
        ConstructorInstance Override(ExplicitArguments arguments);
    }
}