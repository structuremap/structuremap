using System;
using System.Collections.Generic;
using StructureMap.Attributes;
using StructureMap.Graph;
using StructureMap.Interceptors;
using StructureMap.Pipeline;

namespace StructureMap.Configuration.DSL.Expressions
{
    /// <summary>
    /// Represents the parameters for creating instances of a given Type
    /// </summary>
    public class CreatePluginFamilyExpression<PLUGINTYPE>
    {
        private readonly List<Action<PluginFamily>> _alterations = new List<Action<PluginFamily>>();
        private readonly List<Action<PluginGraph>> _children = new List<Action<PluginGraph>>();
        private readonly Type _pluginType;
        private readonly InstanceScope _scope = InstanceScope.PerRequest;

        public CreatePluginFamilyExpression(Registry registry)
        {
            _pluginType = typeof (PLUGINTYPE);

            registry.addExpression(graph =>
            {
                PluginFamily family = graph.FindFamily(_pluginType);
                family.SetScopeTo(_scope);

                _children.ForEach(action => action(graph));
                _alterations.ForEach(action => action(family));
            });
        }

        public CreatePluginFamilyExpression<PLUGINTYPE> AddInstances(params Instance[] instances)
        {
            return alterAndContinue(family =>
            {
                foreach (Instance instance in instances)
                {
                    family.AddInstance(instance);
                }
            });
        }


        private CreatePluginFamilyExpression<PLUGINTYPE> alterAndContinue(Action<PluginFamily> action)
        {
            _alterations.Add(action);
            return this;
        }

        /// <summary>
        /// Sets the default instance of a Type to the definition represented by builder
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public CreatePluginFamilyExpression<PLUGINTYPE> TheDefaultIs(Instance instance)
        {
            return alterAndContinue(family =>
            {
                family.AddInstance(instance);
                family.DefaultInstanceKey = instance.Name;
            });
        }

        public CreatePluginFamilyExpression<PLUGINTYPE> AddInstance(Instance instance)
        {
            return alterAndContinue(family => family.AddInstance(instance));
        }

        /// <summary>
        /// Convenience method that sets the default concrete type of the PluginType.  Type T
        /// can only accept types that do not have any primitive constructor arguments.
        /// StructureMap has to know how to construct all of the constructor argument types.
        /// </summary>
        /// <typeparam name="CONCRETETYPE"></typeparam>
        /// <returns></returns>
        public CreatePluginFamilyExpression<PLUGINTYPE> TheDefaultIsConcreteType<CONCRETETYPE>()
            where CONCRETETYPE : PLUGINTYPE
        {
            ExpressionValidator.ValidatePluggabilityOf(typeof (CONCRETETYPE)).IntoPluginType(_pluginType);

            return alterAndContinue(family =>
            {
                Plugin plugin = family.FindPlugin(typeof (CONCRETETYPE));
                family.DefaultInstanceKey = plugin.ConcreteKey;
            });

            return this;
        }

        /// <summary>
        /// Sets the object creation of the instances of the PluginType.  For example:  PerRequest,
        /// Singleton, ThreadLocal, HttpContext, or Hybrid
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        public CreatePluginFamilyExpression<PLUGINTYPE> CacheBy(InstanceScope scope)
        {
            return alterAndContinue(family => family.SetScopeTo(scope));
        }

        /// <summary>
        /// Convenience method to mark a PluginFamily as a Singleton
        /// </summary>
        /// <returns></returns>
        public CreatePluginFamilyExpression<PLUGINTYPE> AsSingletons()
        {
            _alterations.Add(family => family.SetScopeTo(InstanceScope.Singleton));
            return this;
        }


        public CreatePluginFamilyExpression<PLUGINTYPE> OnCreation(Action<PLUGINTYPE> handler)
        {
            _children.Add(
                graph =>
                {
                    Func<object, object> function = target =>
                    {
                        handler((PLUGINTYPE) target);
                        return target;
                    };

                    PluginTypeInterceptor interceptor = new PluginTypeInterceptor(typeof (PLUGINTYPE), function);
                    graph.InterceptorLibrary.AddInterceptor(interceptor);
                });

            return this;
        }

        public CreatePluginFamilyExpression<PLUGINTYPE> EnrichWith(EnrichmentHandler<PLUGINTYPE> handler)
        {
            _children.Add(
                graph =>
                {
                    Func<object, object> function = target => handler((PLUGINTYPE) target);

                    PluginTypeInterceptor interceptor = new PluginTypeInterceptor(typeof (PLUGINTYPE), function);
                    graph.InterceptorLibrary.AddInterceptor(interceptor);
                });

            return this;
        }

        public CreatePluginFamilyExpression<PLUGINTYPE> AddConcreteType<CONCRETETYPE>()
        {
            return AddConcreteType<CONCRETETYPE>(Guid.NewGuid().ToString());
        }

        public CreatePluginFamilyExpression<PLUGINTYPE> AddConcreteType<CONCRETETYPE>(string instanceName)
        {
            ExpressionValidator.ValidatePluggabilityOf(typeof (CONCRETETYPE)).IntoPluginType(typeof (PLUGINTYPE));

            _alterations.Add(
                family =>
                {
                    ConfiguredInstance instance = new ConfiguredInstance(typeof(CONCRETETYPE)).WithName(instanceName);
                    family.AddInstance(instance);
                }
                );

            return this;
        }

        public CreatePluginFamilyExpression<PLUGINTYPE> InterceptConstructionWith(IBuildInterceptor interceptor)
        {
            _alterations.Add(family => family.AddInterceptor(interceptor));
            return this;
        }

        public CreatePluginFamilyExpression<PLUGINTYPE> AddInstancesFrom(MementoSource source)
        {
            _alterations.Add(family => family.AddMementoSource(source));

            return this;
        }

        public CreatePluginFamilyExpression<PLUGINTYPE> TheDefaultIs(PLUGINTYPE @object)
        {
            return TheDefaultIs(new LiteralInstance(@object));
        }

        public CreatePluginFamilyExpression<PLUGINTYPE> TheDefaultIs(Func<PLUGINTYPE> func)
        {
            ConstructorInstance instance = new ConstructorInstance(() => func());
            return TheDefaultIs(instance);
        }

        public CreatePluginFamilyExpression<PLUGINTYPE> AddInstance(PLUGINTYPE @object)
        {
            LiteralInstance instance = new LiteralInstance(@object);
            return AddInstance(instance);
        }

        public CreatePluginFamilyExpression<PLUGINTYPE> AddInstance(Func<PLUGINTYPE> func)
        {
            ConstructorInstance instance = new ConstructorInstance(() => func());
            return AddInstance(instance);
        }

    }
}