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
        private readonly Func<object, object> _function;
        private readonly Type _pluginType;

        public PluginTypeInterceptor(Type pluginType, Func<object, object> function)
        {
            _pluginType = pluginType;
            _function = function;
        }

        #region TypeInterceptor Members

        public bool MatchesType(Type type)
        {
            return TypeRules.CanBeCast(_pluginType, type);
        }

        public object Process(object target)
        {
            return _function(target);
        }

        #endregion
    }
}