using System;
using StructureMap.Configuration.DSL.Expressions;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.Configuration.DSL
{
    public interface IProfileRegistry
    {
        /// <summary>
        /// All requests For the "TO" types will be filled by fetching the "FROM"
        /// type and casting it to "TO"
        /// GetInstance(typeof(TO)) basically becomes (TO)GetInstance(typeof(FROM))
        /// </summary>
        /// <typeparam name="TFrom"></typeparam>
        /// <typeparam name="TTo"></typeparam>
        void Forward<TFrom, TTo>() where TFrom : class where TTo : class;

        /// <summary>
        /// Expression Builder used to define policies for a PluginType including
        /// Scoping, the Default Instance, and interception.  BuildInstancesOf()
        /// and ForRequestedType() are synonyms
        /// </summary>
        /// <typeparam name="TPluginType"></typeparam>
        /// <param name="lifecycle">Optionally specify the instance scoping for this PluginType</param>
        /// <returns></returns>
        CreatePluginFamilyExpression<TPluginType> For<TPluginType>(ILifecycle lifecycle = null);

        /// <summary>
        /// Expression Builder used to define policies for a PluginType including
        /// Scoping, the Default Instance, and interception.  This method is specifically
        /// meant for registering open generic types
        /// </summary>
        /// <param name="lifecycle">Optionally specify the instance scoping for this PluginType</param>
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
    }

    // TODO -- add a new method for global IInterceptionPolicy
    public interface IRegistry : IProfileRegistry
    {
        /// <summary>
        /// Adds the concreteType as an Instance of the pluginType.  Mostly useful
        /// for conventions
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="concreteType"></param>
        void AddType(Type pluginType, Type concreteType);

        /// <summary>
        /// Adds the concreteType as an Instance of the pluginType with a name.  Mostly
        /// useful for conventions
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
        /// Convenience method.  Equivalent of ForRequestedType[PluginType]().Singletons()
        /// </summary>
        /// <typeparam name="TPluginType"></typeparam>
        /// <returns></returns>
        CreatePluginFamilyExpression<TPluginType> ForSingletonOf<TPluginType>();

        /// <summary>
        /// Shorthand way of saying For(pluginType).Singleton()
        /// </summary>
        /// <param name="pluginType"></param>
        /// <returns></returns>
        GenericFamilyExpression ForSingletonOf(Type pluginType);

        /// <summary>
        /// An alternative way to use CreateProfile that uses ProfileExpression
        /// as a Nested Closure.  This usage will result in cleaner code for 
        /// multiple declarations
        /// </summary>
        /// <param name="profileName"></param>
        /// <param name="action"></param>
        void Profile(string profileName, Action<IProfileRegistry> action);

        /// <summary>
        /// Designates a policy for scanning assemblies to auto
        /// register types
        /// </summary>
        /// <returns></returns>
        void Scan(Action<IAssemblyScanner> action);



        /// <summary>
        /// Advanced Usage Only!  Skips the Registry and goes right to the inner
        /// Semantic Model of StructureMap.  Use with care
        /// </summary>
        /// <param name="configure"></param>
        void Configure(Action<PluginGraph> configure);

        Registry.PoliciesExpression Policies { get; }
    }
}