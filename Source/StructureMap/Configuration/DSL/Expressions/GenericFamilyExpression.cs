using System;
using StructureMap.Attributes;
using StructureMap.Graph;
using StructureMap.Interceptors;
using StructureMap.Pipeline;

namespace StructureMap.Configuration.DSL.Expressions
{
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

        public ConfiguredInstance TheDefaultIsConcreteType(Type concreteType)
        {
            var instance = new ConfiguredInstance(concreteType);
            alterAndContinue(family =>
            {
                family.AddInstance(instance);
                family.DefaultInstanceKey = instance.Name;
            });

            return instance;
        }

        public ConfiguredInstance AddType(Type concreteType)
        {
            var instance = new ConfiguredInstance(concreteType);

            alterAndContinue(family =>
            {
                family.AddInstance(instance);
            });

            return instance;
        }


        private GenericFamilyExpression add(Instance instance)
        {
            return alterAndContinue(family => family.AddInstance(instance));
        }

        public GenericFamilyExpression CacheBy(InstanceScope scope)
        {
            return alterAndContinue(family => family.SetScopeTo(scope));
        }

        public GenericFamilyExpression OnCreation(Action<object> action)
        {
            Func<object, object> func = raw =>
            {
                action(raw);
                return raw;
            };
            return EnrichWith(func);
        }

        public GenericFamilyExpression EnrichWith(Func<object, object> func)
        {
            _registry.addExpression(graph =>
            {
                PluginTypeInterceptor interceptor = new PluginTypeInterceptor(_pluginType, func);
                graph.InterceptorLibrary.AddInterceptor(interceptor);
            });

            return this;
        }


        public GenericFamilyExpression InterceptConstructionWith(IBuildInterceptor interceptor)
        {
            return alterAndContinue(family => family.AddInterceptor(interceptor));
        }

        public GenericFamilyExpression AddConcreteType(Type concreteType)
        {
            return add(new ConfiguredInstance(concreteType));
        }

        public GenericFamilyExpression AddConcreteType(Type concreteType, string instanceName)
        {
            return add(new ConfiguredInstance(concreteType).WithName(instanceName));
        }
    }
}