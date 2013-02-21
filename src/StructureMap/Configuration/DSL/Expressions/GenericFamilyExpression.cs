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

        public GenericFamilyExpression(Type pluginType, InstanceScope? scope, Registry registry)
        {
            _pluginType = pluginType;
            _registry = registry;

            alterAndContinue(f => { });

            if (scope != null)
            {
                alterAndContinue(family => family.SetScopeTo(scope.Value));
            }
        }

        private GenericFamilyExpression alterAndContinue(Action<PluginFamily> action)
        {
            _registry.alter = graph =>
            {
                var family = graph.Families[_pluginType];
                action(family);
            };

            return this;
        }



        /// <summary>
        /// Use this configured Instance as is
        /// </summary>
        /// <param name="instance"></param>
        public void Use(Instance instance)
        {
            alterAndContinue(family => family.SetDefault(instance));
        }

        /// <summary>
        /// Convenience method that sets the default concrete type of the PluginType.  The "concreteType"
        /// can only accept types that do not have any primitive constructor arguments.
        /// StructureMap has to know how to construct all of the constructor argument types.
        /// </summary>
        /// <param name="concreteType"></param>
        /// <returns></returns>
        public ConfiguredInstance Use(Type concreteType)
        {
            var instance = new ConfiguredInstance(concreteType);
            Use(instance);

            return instance;
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

        public ReferencedInstance Use(string instanceKey)
        {
            var instance = new ReferencedInstance(instanceKey);
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
        public ConfiguredInstance Add(Type concreteType)
        {
            var instance = new ConfiguredInstance(concreteType);

            alterAndContinue(family => family.AddInstance(instance));

            return instance;
        }


        public GenericFamilyExpression Add(Instance instance)
        {
            return alterAndContinue(family => family.AddInstance(instance));
        }
        
        /// <summary>
        /// Configure this type as the supplied value
        /// </summary>
        /// <returns></returns>
        public ObjectInstance Add(object @object)
        {
            var instance = new ObjectInstance(@object);
            Add(instance);

            return instance;
        }


        /// <summary>
        /// Register an Action to run against any object of this PluginType immediately after
        /// it is created, but before the new object is passed back to the caller
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public GenericFamilyExpression OnCreationForAll(Action<object> action)
        {
            Func<object, object> func = raw =>
            {
                action(raw);
                return raw;
            };
            return EnrichAllWith(func);
        }

        /// <summary>
        /// Register a Func to run against any object of this PluginType immediately after it is created,
        /// but before the new object is passed back to the caller.  Unlike <see cref="OnCreationForAll">OnCreationForAll()</see>,
        /// EnrichAllWith() gives the the ability to return a different object.  Use this method for runtime AOP
        /// scenarios or to return a decorator.
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public GenericFamilyExpression EnrichAllWith(Func<object, object> func)
        {
            _registry.alter = graph =>
            {
                var interceptor = new PluginTypeInterceptor(_pluginType, (c, o) => func(o));
                graph.InterceptorLibrary.AddInterceptor(interceptor);
            };

            return this;
        }

        /// <summary>
        /// Register a Func to run against any object of this PluginType immediately after it is created,
        /// but before the new object is passed back to the caller.  Unlike <see cref="OnCreationForAll">OnCreationForAll()</see>,
        /// EnrichAllWith() gives the the ability to return a different object.  Use this method for runtime AOP
        /// scenarios or to return a decorator.
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public GenericFamilyExpression EnrichAllWith(Func<IContext, object, object> func)
        {
            _registry.alter = graph =>
            {
                var interceptor = new PluginTypeInterceptor(_pluginType, func);
                graph.InterceptorLibrary.AddInterceptor(interceptor);
            };

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

    }
}