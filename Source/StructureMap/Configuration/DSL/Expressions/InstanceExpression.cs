using System;
using StructureMap.Pipeline;

namespace StructureMap.Configuration.DSL.Expressions
{
    /// <summary>
    /// Expression Builder to define an Instance
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IsExpression<T>
    {
        /// <summary>
        /// Gives you full access to all the different ways to specify an "Instance"
        /// </summary>
        IInstanceExpression<T> Is { get; }

        /// <summary>
        /// Register a previously built Instance.  This provides a "catch all"
        /// method to attach custom Instance objects.  Synonym for Instance()
        /// </summary>
        /// <param name="instance"></param>
        void IsThis(Instance instance);

        /// <summary>
        /// Inject this object directly.  Synonym to Object()
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        LiteralInstance IsThis(T obj);
    }

    public interface ThenItExpression<T>
    {
        IsExpression<T> ThenIt { get; }
    }

    /// <summary>
    /// An Expression Builder to define Instances of a PluginType.
    /// This is mostly used for configuring open generic types
    /// </summary>
    public class GenericIsExpression
    {
        private readonly Action<Instance> _action;

        public GenericIsExpression(Action<Instance> action)
        {
            _action = action;
        }

        /// <summary>
        /// Shortcut to register a Concrete Type as an instance.  This method supports
        /// method chaining to allow you to add constructor and setter arguments for 
        /// the concrete type
        /// </summary>
        /// <param name="concreteType"></param>
        /// <returns></returns>
        public ConfiguredInstance Is(Type concreteType)
        {
            var instance = new ConfiguredInstance(concreteType);
            _action(instance);

            return instance;
        }

        /// <summary>
        /// Shortcut to simply use the Instance with the given name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ReferencedInstance TheInstanceNamed(string name)
        {
            var instance = new ReferencedInstance(name);
            _action(instance);

            return instance;
        }
    }

    /// <summary>
    /// An Expression Builder that is used throughout the Registry DSL to
    /// add and define Instances
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IInstanceExpression<T> : IsExpression<T>
    {
        /// <summary>
        /// Register a previously built Instance.  This provides a "catch all"
        /// method to attach custom Instance objects.  Synonym for IsThis()
        /// </summary>
        /// <param name="instance"></param>
        void Instance(Instance instance);

        /// <summary>
        /// Inject this object directly.  Synonym to IsThis()
        /// </summary>
        /// <param name="theObject"></param>
        /// <returns></returns>
        LiteralInstance Object(T theObject);

        /// <summary>
        /// Build the Instance with the constructor function and setter arguments.  Starts
        /// the definition of a <see cref="SmartInstance{T}">SmartInstance</see>
        /// </summary>
        /// <typeparam name="PLUGGEDTYPE"></typeparam>
        /// <returns></returns>
        SmartInstance<PLUGGEDTYPE> OfConcreteType<PLUGGEDTYPE>() where PLUGGEDTYPE : T;

        /// <summary>
        /// Build the Instance with the constructor function and setter arguments.  Use this
        /// method for open generic types, and favor the generic version of OfConcreteType
        /// for all other types
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        ConfiguredInstance OfConcreteType(Type type);

        /// <summary>
        /// Create an Instance that builds an object by calling a Lambda or
        /// an anonymous delegate with no arguments
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        ConstructorInstance<T> ConstructedBy(Func<T> func);

        /// <summary>
        /// Create an Instance that builds an object by calling a Lambda or
        /// an anonymous delegate with the <see cref="IContext">IContext</see> representing
        /// the current object graph.
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        ConstructorInstance<T> ConstructedBy(Func<IContext, T> func);

        /// <summary>
        /// Use the Instance of this PluginType with the specified name.  This is
        /// generally only used while configuring child dependencies within a deep
        /// object graph
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        ReferencedInstance TheInstanceNamed(string name);

        /// <summary>
        /// Use the default Instance of this PluginType.  This is
        /// generally only used while configuring child dependencies within a deep
        /// object graph
        /// </summary>
        /// <returns></returns>
        DefaultInstance TheDefault();

        /// <summary>
        /// Creates an Instance that stores this object of type T,
        /// and returns a cloned copy of the template.  
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        PrototypeInstance PrototypeOf(T template);

        /// <summary>
        /// Caches template as a serialized byte stream.  Uses deserialization
        /// to create copies when the Instance is built.
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        SerializedInstance SerializedCopyOf(T template);
        
        /// <summary>
        /// Creates an Instance that will load an ASCX user control from the url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        UserControlInstance LoadControlFrom(string url);

        /// <summary>
        /// Creates an Instance according to conditional rules
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        // Conditional object construction
        ConditionalInstance<T> Conditional(Action<ConditionalInstance<T>.ConditionalInstanceExpression> configuration);
    }

    public class InstanceExpression<T> : IInstanceExpression<T>, ThenItExpression<T> 
    {
        private readonly Action<Instance> _action;

        internal InstanceExpression(Action<Instance> action)
        {
            _action = action;
        }

        #region IsExpression<T> Members

        IInstanceExpression<T> IsExpression<T>.Is
        {
            get { return this; }
        }

        public void IsThis(Instance instance)
        {
            returnInstance(instance);
        }

        public LiteralInstance IsThis(T obj)
        {
            return returnInstance(new LiteralInstance(obj));
        }

        #endregion

        public void Instance(Instance instance)
        {
            _action(instance);
        }

        private INSTANCE returnInstance<INSTANCE>(INSTANCE instance) where INSTANCE : Instance
        {
            Instance(instance);
            return instance;
        }

        public SmartInstance<PLUGGEDTYPE> OfConcreteType<PLUGGEDTYPE>() where PLUGGEDTYPE : T
        {
            return returnInstance(new SmartInstance<PLUGGEDTYPE>());
        }

        public ConfiguredInstance OfConcreteType(Type type)
        {
            return returnInstance(new ConfiguredInstance(type));
        }

        public LiteralInstance Object(T theObject)
        {
            return returnInstance(new LiteralInstance(theObject));
        }

        public ReferencedInstance TheInstanceNamed(string name)
        {
            return returnInstance(new ReferencedInstance(name));
        }

        public DefaultInstance TheDefault()
        {
            return returnInstance(new DefaultInstance());
        }

        public ConstructorInstance<T> ConstructedBy(Func<T> func)
        {
            return returnInstance(new ConstructorInstance<T>(func));
        }

        public ConstructorInstance<T> ConstructedBy(Func<IContext, T> func)
        {
            return returnInstance(new ConstructorInstance<T>(func));
        }

        public PrototypeInstance PrototypeOf(T template)
        {
            return returnInstance(new PrototypeInstance((ICloneable) template));
        }

        public SerializedInstance SerializedCopyOf(T template)
        {
            return returnInstance(new SerializedInstance(template));
        }

        public UserControlInstance LoadControlFrom(string url)
        {
            return returnInstance(new UserControlInstance(url));
        }

        public ConditionalInstance<T> Conditional(Action<ConditionalInstance<T>.ConditionalInstanceExpression> configuration)
        {
            return returnInstance(new ConditionalInstance<T>(configuration));
        }


        IsExpression<T> ThenItExpression<T>.ThenIt
        {
            get { return this; }
        }
    }
}