using System;
using System.Linq.Expressions;
using StructureMap.Building.Interception;
using StructureMap.Graph;
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

        public GenericFamilyExpression(Type pluginType, ILifecycle scope, Registry registry)
        {
            _pluginType = pluginType;
            _registry = registry;

            alterAndContinue(f => { });

            if (scope != null)
            {
                alterAndContinue(family => family.SetLifecycleTo(scope));
            }
        }

        private GenericFamilyExpression alterAndContinue(Action<PluginFamily> action)
        {
            _registry.alter = graph => {
                var family = graph.FindExistingOrCreateFamily(_pluginType);

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

        /// <summary>
        /// Specify the "on missing named instance" configuration for this
        /// PluginType
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public GenericFamilyExpression MissingNamedInstanceIs(Instance instance)
        {
            alterAndContinue(family => family.MissingInstance = instance);
            return this;
        }

        /// <summary>
        /// Register an Instance constructed by a Lambda Expression using IContext
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public LambdaInstance<object> Use(Expression<Func<IContext, object>> func)
        {
            var instance = new LambdaInstance<object>(func);
            Use(instance);

            return instance;
        }

        /// <summary>
        /// Register an Instance constructed by a Func that uses IContex
        /// </summary>
        /// <param name="description">User friendly diagnostic description</param>
        /// <param name="func"></param>
        /// <returns></returns>
        public LambdaInstance<object> Use(string description, Func<IContext, object> func)
        {
            var instance = new LambdaInstance<object>(description, func);
            Use(instance);

            return instance;
        }

        /// <summary>
        /// Adds an additional Instance constructed by a Lambda Expression using IContext
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public LambdaInstance<object> Add(Expression<Func<IContext, object>> func)
        {
            var instance = new LambdaInstance<object>(func);
            Add(instance);

            return instance;
        }

        /// <summary>
        /// Adds an additional Instance constructed by a Func using IContext
        /// </summary>
        /// <param name="description">User friendly description for diagnostic purposes</param>
        /// <param name="func"></param>
        /// <returns></returns>
        public LambdaInstance<object> Add(string description, Func<IContext, object> func)
        {
            var instance = new LambdaInstance<object>(description, func);
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
        /// Makes a previously registered Instance with the name 'instanceKey'
        /// the default Instance for this PluginType
        /// </summary>
        /// <param name="instanceKey"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Adds an additional Instance against this PluginType
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
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
        /// Assign a lifecycle to the PluginFamily
        /// </summary>
        /// <param name="lifecycle"></param>
        /// <returns></returns>
        public GenericFamilyExpression LifecycleIs(ILifecycle lifecycle)
        {
            return alterAndContinue(family => family.SetLifecycleTo(lifecycle));
        }

        /// <summary>
        /// Convenience method to mark a PluginFamily as a Singleton
        /// </summary>
        /// <returns></returns>
        public GenericFamilyExpression Singleton()
        {
            return LifecycleIs(Lifecycles.Singleton);
        }

        /// <summary>
        /// Applies a decorator type to all Instances that return a type that can be cast to this PluginType
        /// </summary>
        /// <param name="decoratorType"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public ConfiguredInstance DecorateAllWith(Type decoratorType, Func<Instance, bool> filter = null)
        {
            var instance = new ConfiguredInstance(decoratorType);
            var policy = new DecoratorPolicy(_pluginType, instance, filter);

            _registry.alter = graph => graph.Policies.Add(policy);

            return instance;
        }

        /// <summary>
        /// Removes any and all previously registered instance from this
        /// plugin type
        /// </summary>
        /// <returns></returns>
        public GenericFamilyExpression ClearAll()
        {
            alterAndContinue(family => family.RemoveAll());
            return this;
        }

        /// <summary>
        /// A general purpose method to configure the underlying
        /// PluginFamily for this type
        /// </summary>
        /// <param name="configure"></param>
        public void Configure(Action<PluginFamily> configure)
        {
            alterAndContinue(configure);
        }
    }
}