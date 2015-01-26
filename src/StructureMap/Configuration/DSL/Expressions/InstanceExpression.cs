using System;
using System.Linq.Expressions;
using StructureMap.Pipeline;
using StructureMap.TypeRules;

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
        ObjectInstance<TReturned, T> IsThis<TReturned>(TReturned obj) where TReturned : class, T;
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
        ObjectInstance<TReturned, T> Object<TReturned>(TReturned theObject) where TReturned : class, T;

        /// <summary>
        /// Build the Instance with the constructor function and setter arguments.  Starts
        /// the definition of a <see cref="SmartInstance{T}">SmartInstance</see>
        /// </summary>
        /// <typeparam name="TPluggedType"></typeparam>
        /// <returns></returns>
        SmartInstance<TPluggedType, T> Type<TPluggedType>() where TPluggedType : T;

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
        LambdaInstance<TReturned, T> ConstructedBy<TReturned>(Expression<Func<TReturned>> func) where TReturned : T;

        /// <summary>
        /// Create an Instance that builds an object by calling a Lambda or
        /// an anonymous delegate with no arguments
        /// </summary>
        /// <param name="func"></param>
        /// <param name="description">Diagnostic description of func</param>
        /// <returns></returns>
        LambdaInstance<TReturned, T> ConstructedBy<TReturned>(string description, Func<TReturned> func) where TReturned : T;

        /// <summary>
        /// Create an Instance that builds an object by calling a Lambda or
        /// an anonymous delegate with the <see cref="IContext">IContext</see> representing
        /// the current object graph.
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        LambdaInstance<TReturned, T> ConstructedBy<TReturned>(Expression<Func<IContext, TReturned>> func) where TReturned : T;

        /// <summary>
        /// Create an Instance that builds an object by calling a Lambda or
        /// an anonymous delegate with the <see cref="IContext">IContext</see> representing
        /// the current object graph.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="description">Diagnostic description of the func</param>
        /// <returns></returns>
        LambdaInstance<TReturned, T> ConstructedBy<TReturned>(string description, Func<IContext, TReturned> func) where TReturned : T;

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

        /// <summary>
        /// Use the specified Instance as the inline dependency
        /// </summary>
        /// <param name="instance"></param>
        public void IsThis(Instance instance)
        {
            returnInstance(instance);
        }

        /// <summary>
        /// Use a specific object as the inline dependency
        /// </summary>
        /// <typeparam name="TReturned"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public ObjectInstance<TReturned, T> IsThis<TReturned>(TReturned obj) where TReturned : class, T
        {
            return returnInstance(new ObjectInstance<TReturned, T>(obj));
        }

        #endregion

        public void Instance(Instance instance)
        {
            _action(instance);
        }

        public SmartInstance<TPluggedType, T> Type<TPluggedType>() where TPluggedType : T
        {
            if (!typeof (TPluggedType).IsConcrete())
            {
                throw new InvalidOperationException("This class can only be created for concrete TPluginType types");
            }

            return returnInstance(new SmartInstance<TPluggedType, T>());
        }

        public ConfiguredInstance Type(Type type)
        {
            return returnInstance(new ConfiguredInstance(type));
        }

        public ObjectInstance<TReturned, T> Object<TReturned>(TReturned theObject) where TReturned : class, T
        {
            return returnInstance(new ObjectInstance<TReturned, T>(theObject));
        }

        public ReferencedInstance TheInstanceNamed(string name)
        {
            return returnInstance(new ReferencedInstance(name));
        }

        public DefaultInstance TheDefault()
        {
            return returnInstance(new DefaultInstance());
        }

        public LambdaInstance<TReturned, T> ConstructedBy<TReturned>(Expression<Func<TReturned>> func) where TReturned : T
        {
            return returnInstance(new LambdaInstance<TReturned, T>(func));
        }

        public LambdaInstance<TReturned, T> ConstructedBy<TReturned>(string description, Func<TReturned> func) where TReturned : T
        {
            return returnInstance(new LambdaInstance<TReturned, T>(description, func));
        }

        public LambdaInstance<TReturned, T> ConstructedBy<TReturned>(Expression<Func<IContext, TReturned>> func) where TReturned : T
        {
            return returnInstance(new LambdaInstance<TReturned, T>(func));
        }

        public LambdaInstance<TReturned, T> ConstructedBy<TReturned>(string description, Func<IContext, TReturned> func) where TReturned : T
        {
            return returnInstance(new LambdaInstance<TReturned, T>(description, func));
        }

        private TInstance returnInstance<TInstance>(TInstance instance) where TInstance : Instance
        {
            Instance(instance);
            return instance;
        }
    }
}