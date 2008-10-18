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

        public CreatePluginFamilyExpression(Registry registry)
        {
            _pluginType = typeof (PLUGINTYPE);

            registry.addExpression(graph =>
            {
                PluginFamily family = graph.FindFamily(_pluginType);

                _children.ForEach(action => action(graph));
                _alterations.ForEach(action => action(family));
            });
        }

        public IsExpression<PLUGINTYPE> TheDefault
        {
            get { return new InstanceExpression<PLUGINTYPE>(i => registerDefault(i)); }
        }

        public CreatePluginFamilyExpression<PLUGINTYPE> AddInstances(Action<InstanceExpression<PLUGINTYPE>> action)
        {
            var list = new List<Instance>();

            var child = new InstanceExpression<PLUGINTYPE>(i => list.Add(i));
            action(child);

            return alterAndContinue(family =>
            {
                foreach (Instance instance in list)
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
        /// Convenience method that sets the default concrete type of the PluginType.  Type T
        /// can only accept types that do not have any primitive constructor arguments.
        /// StructureMap has to know how to construct all of the constructor argument types.
        /// </summary>
        /// <typeparam name="CONCRETETYPE"></typeparam>
        /// <returns></returns>
        public CreatePluginFamilyExpression<PLUGINTYPE> TheDefaultIsConcreteType<CONCRETETYPE>()
            where CONCRETETYPE : PLUGINTYPE
        {
            Type concreteType = typeof (CONCRETETYPE);

            ExpressionValidator.ValidatePluggabilityOf(concreteType).IntoPluginType(_pluginType);

            if (!PluginCache.GetPlugin(concreteType).CanBeAutoFilled)
            {
                throw new StructureMapException(231);
            }
            
            return alterAndContinue(family =>
            {
                ConfiguredInstance instance =
                    new ConfiguredInstance(concreteType).WithName(concreteType.AssemblyQualifiedName);
                family.AddInstance(instance);
                family.DefaultInstanceKey = instance.Name;
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

                    var interceptor = new PluginTypeInterceptor(typeof (PLUGINTYPE), function);
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

                    var interceptor = new PluginTypeInterceptor(typeof (PLUGINTYPE), function);
                    graph.InterceptorLibrary.AddInterceptor(interceptor);
                });

            return this;
        }


        public CreatePluginFamilyExpression<PLUGINTYPE> AddConcreteType<PLUGGEDTYPE>()
        {
            _alterations.Add(family =>
            {
                string name = PluginCache.GetPlugin(typeof (PLUGGEDTYPE)).ConcreteKey;
                SmartInstance<PLUGGEDTYPE> instance = new SmartInstance<PLUGGEDTYPE>().WithName(name);
                family.AddInstance(instance);
            });

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

        private void registerDefault(Instance instance)
        {
            _alterations.Add(family =>
            {
                family.AddInstance(instance);
                family.DefaultInstanceKey = instance.Name;
            });
        }
    }
}