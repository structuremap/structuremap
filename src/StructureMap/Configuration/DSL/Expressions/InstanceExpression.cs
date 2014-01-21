using System;
using System.Linq.Expressions;
using StructureMap.Pipeline;

namespace StructureMap.Configuration.DSL.Expressions
{
    // TODO -- need to add Xml comments

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
        ObjectInstance IsThis(T obj);
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
        ObjectInstance Object(T theObject);


        /// <summary>
        /// Build the Instance with the constructor function and setter arguments.  Starts
        /// the definition of a <see cref="SmartInstance{T}">SmartInstance</see>
        /// </summary>
        /// <typeparam name="TPluggedType"></typeparam>
        /// <returns></returns>
        SmartInstance<TPluggedType> Type<TPluggedType>();

        /// <summary>
        /// Build the Instance with the constructor function and setter arguments.  Use this
        /// method for open generic types, and favor the generic version of Type()
        /// for all other types
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        ConfiguredInstance Type(Type type);

        /// <summary>
        /// Create an Instance that builds an object by calling a Lambda or
        /// an anonymous delegate with no arguments
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        LambdaInstance<T> ConstructedBy(Expression<Func<T>> func);

        /// <summary>
        /// Create an Instance that builds an object by calling a Lambda or
        /// an anonymous delegate with no arguments
        /// </summary>
        /// <param name="func"></param>
        /// <param name="description">Diagnostic description of func</param>
        /// <returns></returns>
        LambdaInstance<T> ConstructedBy(string description, Func<T> func);

        /// <summary>
        /// Create an Instance that builds an object by calling a Lambda or
        /// an anonymous delegate with the <see cref="IContext">IContext</see> representing
        /// the current object graph.
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        LambdaInstance<T> ConstructedBy(Expression<Func<IContext, T>> func);

        /// <summary>
        /// Create an Instance that builds an object by calling a Lambda or
        /// an anonymous delegate with the <see cref="IContext">IContext</see> representing
        /// the current object graph.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="description">Diagnostic description of the func</param>
        /// <returns></returns>
        LambdaInstance<T> ConstructedBy(string description, Func<IContext, T> func);

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
    }

    public class InstanceExpression<T> : IInstanceExpression<T>
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

        public ObjectInstance IsThis(T obj)
        {
            return returnInstance(new ObjectInstance(obj));
        }

        #endregion

        public void Instance(Instance instance)
        {
            _action(instance);
        }

        public SmartInstance<TTPluggedType> Type<TTPluggedType>()
        {
            // TODO -- this needs to blow up if it's not a concrete type

            return returnInstance(new SmartInstance<TTPluggedType>());
        }

        public ConfiguredInstance Type(Type type)
        {
            return returnInstance(new ConfiguredInstance(type));
        }

        public ObjectInstance Object(T theObject)
        {
            return returnInstance(new ObjectInstance(theObject));
        }

        public ReferencedInstance TheInstanceNamed(string name)
        {
            return returnInstance(new ReferencedInstance(name));
        }

        public DefaultInstance TheDefault()
        {
            return returnInstance(new DefaultInstance());
        }

        public LambdaInstance<T> ConstructedBy(Expression<Func<T>> func)
        {
            return returnInstance(new LambdaInstance<T>(func));
        }

        public LambdaInstance<T> ConstructedBy(string description, Func<T> func)
        {
            return returnInstance(new LambdaInstance<T>(description, func));
        }

        public LambdaInstance<T> ConstructedBy(Expression<Func<IContext, T>> func)
        {
            return returnInstance(new LambdaInstance<T>(func));
        }

        public LambdaInstance<T> ConstructedBy(string description, Func<IContext, T> func)
        {
            return returnInstance(new LambdaInstance<T>(description, func));
        }

        private TInstance returnInstance<TInstance>(TInstance instance) where TInstance : Instance
        {
            Instance(instance);
            return instance;
        }
    }
}