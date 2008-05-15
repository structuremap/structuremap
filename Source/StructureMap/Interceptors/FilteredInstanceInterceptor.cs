using System;
using StructureMap.Graph;

namespace StructureMap.Interceptors
{
    public interface TypeInterceptor : InstanceInterceptor
    {
        bool MatchesType(Type type);
    }

    public class PluginTypeInterceptor : TypeInterceptor
    {
        private readonly Type _pluginType;
        private readonly InterceptionFunction _function;

        public PluginTypeInterceptor(Type pluginType, InterceptionFunction function)
        {
            _pluginType = pluginType;
            _function = function;
        }

        public bool MatchesType(Type type)
        {
            return TypeRules.CanBeCast(_pluginType, type);
        }

        public object Process(object target)
        {
            return _function(target);
        }
    }
}