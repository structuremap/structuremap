using System;
using System.Collections.Generic;
using System.Reflection;
using StructureMap.Pipeline;
using StructureMap.TypeRules;

namespace StructureMap.Building.Interception
{
    public class DecoratorPolicy : IInterceptorPolicy
    {
        private readonly Type _pluginType;
        private readonly ConfiguredInstance _instance;
        private readonly Func<Instance, bool> _filter;

        public DecoratorPolicy(Type pluginType, Type pluggedType, Func<Instance, bool> filter = null)
            : this(pluginType, new ConfiguredInstance(pluggedType), filter)
        {
        }

        public DecoratorPolicy(Type pluginType, ConfiguredInstance instance, Func<Instance, bool> filter = null)
        {
            _pluginType = pluginType;
            _instance = instance;
            _filter = filter ?? (i => true);
        }

        public string Description
        {
            get
            {
                return "Decorate with '{0}' on any plugin type that can be cast to {1}".ToFormat(_instance.Description,
                    _instance.PluggedType.GetFullName());
            }
        }

        public IEnumerable<IInterceptor> DetermineInterceptors(Type pluginType, Instance instance)
        {
            if (!_filter(instance))
            {
                yield break;
            }

            if (pluginType == _pluginType)
            {
                yield return new DecoratorInterceptor(pluginType, _instance);
            }
            else if (_pluginType.IsOpenGeneric() && pluginType.GetTypeInfo().IsGenericType &&
                     pluginType.GetTypeInfo().GetGenericTypeDefinition() == _pluginType)
            {
                var parameters = pluginType.GetGenericArguments();
                var closedInstance = _instance.CloseType(parameters) as IConfiguredInstance;

                yield return new DecoratorInterceptor(pluginType, closedInstance);
            }
        }
    }
}