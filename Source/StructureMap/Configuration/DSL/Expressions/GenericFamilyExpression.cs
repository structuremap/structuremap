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
            _registry.addExpression(delegate(PluginGraph graph)
            {
                PluginFamily family = graph.FindFamily(_pluginType);
                action(family);
            });

            return this;
        }

        public GenericFamilyExpression TheDefaultIsConcreteType(Type concreteType)
        {
            ConfiguredInstance instance = new ConfiguredInstance(concreteType);
            return TheDefaultIs(instance);
        }

        public GenericFamilyExpression TheDefaultIs(Instance instance)
        {
            return alterAndContinue(delegate(PluginFamily family)
            {
                family.AddInstance(instance);
                family.DefaultInstanceKey = instance.Name;
            });
        }

        public GenericFamilyExpression TheDefaultIs(Func<object> func)
        {
            ConstructorInstance instance = new ConstructorInstance(func);
            return TheDefaultIs(instance);
        }

        public GenericFamilyExpression AddInstance(Instance instance)
        {
            return alterAndContinue(delegate(PluginFamily family) { family.AddInstance(instance); });
        }

        public GenericFamilyExpression CacheBy(InstanceScope scope)
        {
            return alterAndContinue(delegate(PluginFamily family) { family.SetScopeTo(scope); });
        }

        public GenericFamilyExpression OnCreation(Action<object> action)
        {
            Func<object, object> func = delegate(object raw)
            {
                action(raw);
                return raw;
            };
            return EnrichWith(func);
        }

        public GenericFamilyExpression EnrichWith(Func<object, object> func)
        {
            _registry.addExpression(delegate(PluginGraph graph)
            {
                PluginTypeInterceptor interceptor = new PluginTypeInterceptor(_pluginType, func);
                graph.InterceptorLibrary.AddInterceptor(interceptor);
            });

            return this;
        }


        public GenericFamilyExpression InterceptConstructionWith(IBuildInterceptor interceptor)
        {
            return alterAndContinue(delegate(PluginFamily family)
            {
                family.AddInterceptor(interceptor);
            });
        }

        public GenericFamilyExpression AddConcreteType(Type concreteType)
        {
            return AddInstance(new ConfiguredInstance(concreteType));
        }

        public GenericFamilyExpression AddConcreteType(Type concreteType, string instanceName)
        {
            return AddInstance(new ConfiguredInstance(concreteType).WithName(instanceName));
        }
    }
}