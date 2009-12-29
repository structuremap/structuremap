using System;
using StructureMap.Graph;
using StructureMap.Interceptors;
using StructureMap.Pipeline;

namespace StructureMap.Configuration.DSL.Expressions
{
    /// <summary>
    /// Expression Builder that has grammars for defining policies at the 
    /// PluginType level.  This expression is used for registering 
    /// open generic types
    /// </summary>
    public class GenericFamilyExpression
    {
        private readonly Type _pluginType;
        private readonly Registry _registry;

        public GenericFamilyExpression(Type pluginType, Registry registry)
        {
            _pluginType = pluginType;
            _registry = registry;
        }

        private GenericFamilyExpression alterAndContinue(Action<PluginFamily> action)
        {
            _registry.addExpression(graph =>
            {
                PluginFamily family = graph.FindFamily(_pluginType);
                action(family);
            });

            return this;
        }

        /// <summary>
        /// Convenience method that sets the default concrete type of the PluginType.  The "concreteType"
        /// can only accept types that do not have any primitive constructor arguments.
        /// StructureMap has to know how to construct all of the constructor argument types.
        /// </summary>
        /// <param name="concreteType"></param>
        /// <returns></returns>
        [Obsolete("Change to Use(concreteType)")]
        public ConfiguredInstance TheDefaultIsConcreteType(Type concreteType)
        {
            var instance = new ConfiguredInstance(concreteType);
            Use(instance);

            return instance;
        }

        /// <summary>
        /// Use this configured Instance as is
        /// </summary>
        /// <param name="instance"></param>
        public void Use(Instance instance)
        {
            alterAndContinue(family =>
            {
                family.AddInstance(instance);
                family.DefaultInstanceKey = instance.Name;
            });
        }

        /// <summary>
        /// Shorter way to call TheDefaultIsConcreteType
        /// </summary>
        /// <param name="concreteType"></param>
        /// <returns></returns>
        public ConfiguredInstance Use(Type concreteType)
        {
            return TheDefaultIsConcreteType(concreteType);
        }


        public LambdaInstance<object> Use(Func<IContext, object> func)
        {
            var instance = new LambdaInstance<object>(func);
            Use(instance);

            return instance;
        }

        public LambdaInstance<object> Add(Func<IContext, object> func)
        {
            var instance = new LambdaInstance<object>(func);
            Add(instance);

            return instance;
        }

        /// <summary>
        /// Shortcut to add a value by type
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ObjectInstance Use(object value)
        {
            var instance = new ObjectInstance(value);
            Use(instance);

            return instance;
        }


        /// <summary>
        /// Shortcut method to add an additional Instance to this Plugin Type
        /// as just a Concrete Type.  This will only work if the Concrete Type
        /// has no primitive constructor or mandatory Setter arguments.
        /// </summary>
        /// <param name="concreteType"></param>
        /// <returns></returns>
        [Obsolete("Switch to Add()")]
        public ConfiguredInstance AddType(Type concreteType)
        {
            var instance = new ConfiguredInstance(concreteType);

            alterAndContinue(family => { family.AddInstance(instance); });

            return instance;
        }

        /// <summary>
        /// Shortcut method to add an additional Instance to this Plugin Type
        /// as just a Concrete Type.  This will only work if the Concrete Type
        /// has no primitive constructor or mandatory Setter arguments.
        /// </summary>
        /// <param name="concreteType"></param>
        /// <returns></returns>
        public ConfiguredInstance Add(Type concreteType)
        {
            return AddType(concreteType);
        }


        public GenericFamilyExpression Add(Instance instance)
        {
            return alterAndContinue(family => family.AddInstance(instance));
        }
        
        /// <summary>
        /// Configure this type as the supplied value
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public ObjectInstance Add(object @object)
        {
            var instance = new ObjectInstance(@object);
            Add(instance);

            return instance;
        }


        /// <summary>
        /// Sets the object creation of the instances of the PluginType.  For example:  PerRequest,
        /// Singleton, ThreadLocal, HttpContext, or Hybrid
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        [Obsolete("Change to LifecycleIs()")]
        public GenericFamilyExpression CacheBy(InstanceScope scope)
        {
            return alterAndContinue(family => family.SetScopeTo(scope));
        }

        /// <summary>
        /// Register an Action to run against any object of this PluginType immediately after
        /// it is created, but before the new object is passed back to the caller
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public GenericFamilyExpression OnCreation(Action<object> action)
        {
            Func<object, object> func = raw =>
            {
                action(raw);
                return raw;
            };
            return EnrichWith(func);
        }

        /// <summary>
        /// Register a Func to run against any object of this PluginType immediately after it is created,
        /// but before the new object is passed back to the caller.  Unlike <see cref="OnCreation">OnCreation()</see>,
        /// EnrichWith() gives the the ability to return a different object.  Use this method for runtime AOP
        /// scenarios or to return a decorator.
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public GenericFamilyExpression EnrichWith(Func<object, object> func)
        {
            _registry.addExpression(graph =>
            {
                var interceptor = new PluginTypeInterceptor(_pluginType, (c, o) => func(o));
                graph.InterceptorLibrary.AddInterceptor(interceptor);
            });

            return this;
        }

        /// <summary>
        /// Register a Func to run against any object of this PluginType immediately after it is created,
        /// but before the new object is passed back to the caller.  Unlike <see cref="OnCreation">OnCreation()</see>,
        /// EnrichWith() gives the the ability to return a different object.  Use this method for runtime AOP
        /// scenarios or to return a decorator.
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public GenericFamilyExpression EnrichWith(Func<IContext, object, object> func)
        {
            _registry.addExpression(graph =>
            {
                var interceptor = new PluginTypeInterceptor(_pluginType, func);
                graph.InterceptorLibrary.AddInterceptor(interceptor);
            });

            return this;
        }

        /// <summary>
        /// Registers an IBuildInterceptor for this Plugin Type that executes before
        /// any object of this PluginType is created.  IBuildInterceptor's can be
        /// used to create a custom scope
        /// </summary>
        /// <param name="lifecycle"></param>
        /// <returns></returns>
        public GenericFamilyExpression LifecycleIs(ILifecycle lifecycle)
        {
            return alterAndContinue(family => family.SetScopeTo(lifecycle));
        }

        /// <summary>
        /// Convenience method to mark a PluginFamily as a Singleton
        /// </summary>
        /// <returns></returns>
        public GenericFamilyExpression Singleton()
        {
            return LifecycleIs(InstanceScope.Singleton);
        }


        /// <summary>
        /// Convenience method to mark a PluginFamily as a Hybrid lifecycle
        /// </summary>
        /// <returns></returns>
        public GenericFamilyExpression HybridHttpOrThreadLocalScoped()
        {
            return LifecycleIs(InstanceScope.Hybrid);
        }

        /// <summary>
        /// Convenience method to mark a PluginFamily as HttpContext scoped
        /// </summary>
        /// <returns></returns>
        public GenericFamilyExpression HttpContextScoped()
        {
            return LifecycleIs(InstanceScope.HttpContext);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lifecycle"></param>
        /// <returns></returns>
        public GenericFamilyExpression LifecycleIs(InstanceScope lifecycle)
        {
            return alterAndContinue(family => family.SetScopeTo(lifecycle));
        }


        /// <summary>
        /// Shortcut method to add an additional Instance to this Plugin Type
        /// as just a Concrete Type.  You can also chain other declarations after
        /// this method to add constructor and setter arguments
        /// </summary>
        /// <param name="concreteType"></param>
        /// <returns></returns>
        [Obsolete("Change to Add(Type)")]
        public GenericFamilyExpression AddConcreteType(Type concreteType)
        {
            var instance = new ConfiguredInstance(concreteType);
            return Add(instance);
        }


        /// <summary>
        /// Shortcut method to add an additional Instance to this Plugin Type
        /// as just a Concrete Type by a specified name.  You can also chain other declarations after
        /// this method to add constructor and setter arguments
        /// </summary>
        /// <param name="concreteType"></param>
        /// <param name="instanceName"></param>
        /// <returns></returns>
        [Obsolete("Change to Add(Type, [name)]")]
        public GenericFamilyExpression AddConcreteType(Type concreteType, string instanceName)
        {
            return Add(new ConfiguredInstance(concreteType).WithName(instanceName));
        }
    }
}