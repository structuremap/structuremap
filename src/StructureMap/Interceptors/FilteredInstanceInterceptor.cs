using System;
using StructureMap.TypeRules;

namespace StructureMap.Interceptors
{
    /// <summary>
    /// A TypeInterceptor that is only applied if the MatchesType()
    /// method is true for a given Type
    /// </summary>
    public interface TypeInterceptor : InstanceInterceptor
    {
        /// <summary>
        /// Does this TypeInterceptor apply to the given type?
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        bool MatchesType(Type type);
    }

    /// <summary>
    /// A TypeInterceptor that always applies to all Instances of a given Plugin Type
    /// </summary>
    [Obsolete]
    public class PluginTypeInterceptor : TypeInterceptor
    {
        private readonly Func<IBuildSession, object, object> _function;
        private readonly Type _pluginType;

        public PluginTypeInterceptor(Type pluginType, Func<IBuildSession, object, object> function)
        {
            _pluginType = pluginType;
            _function = function;
        }

        #region TypeInterceptor Members

        public bool MatchesType(Type type)
        {
            return type.CanBeCastTo(_pluginType);
        }

        public object Process(object target, IBuildSession session)
        {
            return _function(session, target);
        }

        #endregion
    }
}