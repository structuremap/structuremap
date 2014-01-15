using System;
using StructureMap.Configuration.DSL.Expressions;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.Configuration.DSL
{
    // TODO -- add a new method for global IInterceptionPolicy
    public interface IRegistry
    {
        /// <summary>
        /// Adds the concreteType as an Instance of the pluginType
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="concreteType"></param>
        void AddType(Type pluginType, Type concreteType);

        /// <summary>
        /// Adds the concreteType as an Instance of the pluginType with a name
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="concreteType"></param>
        /// <param name="name"></param>
        void AddType(Type pluginType, Type concreteType, string name);

        /// <summary>
        /// Imports the configuration from another registry into this registry.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void IncludeRegistry<T>() where T : Registry, new();

        /// <summary>
        /// Imports the configuration from another registry into this registry.
        /// </summary>
        /// <param name="registry"></param>
        void IncludeRegistry(Registry registry);

        /// <summary>
        /// This method is a shortcut for specifying the default constructor and 
        /// setter arguments for a ConcreteType.  ForConcreteType is shorthand for:
        /// For[T]().Use[T].**************
        /// when the PluginType and ConcreteType are the same Type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Registry.BuildWithExpression<T> ForConcreteType<T>();

        /// <summary>
        /// Convenience method.  Equivalent of For[PluginType]().AsSingletons()
        /// </summary>
        /// <typeparam name="TPluginType"></typeparam>
        /// <returns></returns>
        CreatePluginFamilyExpression<TPluginType> ForSingletonOf<TPluginType>();

        /// <summary>
        /// An alternative way to use CreateProfile that uses ProfileExpression
        /// as a Nested Closure.  This usage will result in cleaner code for 
        /// multiple declarations
        /// </summary>
        /// <param name="profileName"></param>
        /// <param name="action"></param>
        void Profile(string profileName, Action<Registry> action);

        /// <summary>
        /// Designates a policy for scanning assemblies to auto
        /// register types
        /// </summary>
        /// <returns></returns>
        void Scan(Action<IAssemblyScanner> action);

        /// <summary>
        /// Directs StructureMap to always inject dependencies into any and all public Setter properties
        /// of the type TPluginType.
        /// </summary>
        /// <typeparam name="TPluginType"></typeparam>
        /// <returns></returns>
        CreatePluginFamilyExpression<TPluginType> FillAllPropertiesOfType<TPluginType>();

        /// <summary>
        /// Creates automatic "policies" for which public setters are considered mandatory
        /// properties by StructureMap that will be "setter injected" as part of the 
        /// construction process.
        /// </summary>
        /// <param name="action"></param>
        void SetAllProperties(Action<SetterConvention> action);

        /// <summary>
        /// All requests For the "TO" types will be filled by fetching the "FROM"
        /// type and casting it to "TO"
        /// GetInstance(typeof(TO)) basically becomes (TO)GetInstance(typeof(FROM))
        /// </summary>
        /// <typeparam name="FROM"></typeparam>
        /// <typeparam name="TO"></typeparam>
        void Forward<FROM, TO>() where FROM : class where TO : class;

        /// <summary>
        /// Shorthand for ForRequestedType<TPluginType>()
        /// </summary>
        /// <typeparam name="TPluginType"></typeparam>
        /// <returns></returns>
        CreatePluginFamilyExpression<TPluginType> For<TPluginType>(ILifecycle lifecycle = null);

        /// <summary>
        /// Shorthand for ForRequestedType(pluginType)
        /// </summary>
        /// <param name="pluginType"></param>
        /// <returns></returns>
        GenericFamilyExpression For(Type pluginType, ILifecycle lifecycle = null);

        /// <summary>
        /// Shortcut to make StructureMap return the default object of U casted to T
        /// whenever T is requested.  I.e.:
        /// For<T>().TheDefault.Is.ConstructedBy(c => c.GetInstance<U>() as T);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <returns></returns>
        LambdaInstance<T> Redirect<T, U>() where T : class where U : class;

        /// <summary>
        /// Advanced Usage Only!  Skips the Registry and goes right to the inner
        /// Semantic Model of StructureMap.  Use with care
        /// </summary>
        /// <param name="configure"></param>
        void Configure(Action<PluginGraph> configure);

        /// <summary>
        /// Gives a <see cref="IPluginGraphConfiguration"/> the possibility to interact with the current <see cref="PluginGraphBuilder"/>
        /// via <see cref="IPluginGraphConfiguration.Register"/>.
        /// </summary>
        void RegisterPluginGraphConfiguration<T>() where T : IPluginGraphConfiguration, new();

        /// <summary>
        /// See <see cref="Registry.RegisterPluginGraphConfiguration{T}"/>
        /// </summary>
        void RegisterPluginGraphConfiguration(IPluginGraphConfiguration pluginGraphConfig);

        /// <summary>
        /// Gives a <see cref="IPluginGraphConfiguration"/> the possibility to interact with the resulting <see cref="PluginGraph"/>,
        /// i.e. as opposed to <see cref="Registry.RegisterPluginGraphConfiguration"/>, the PluginGraph is built, and the provided
        /// PluginGraph config obtains access to saig graph.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void ConfigurePluginGraph<T>() where T : IPluginGraphConfiguration, new();

        /// <summary>
        /// <see cref="Registry.ConfigurePluginGraph{T}"/>
        /// </summary>
        void ConfigurePluginGraph(IPluginGraphConfiguration pluginGraphConfig);
    }
}