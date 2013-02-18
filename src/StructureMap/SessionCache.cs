using System;
using System.Collections.Generic;
using StructureMap.Pipeline;

namespace StructureMap
{
    public interface ISessionCache
    {
        object GetDefault(Type pluginType, IPipelineGraph pipelineGraph);
        object GetObject(Type pluginType, Instance instance);
    }

    public class SessionCache : ISessionCache
    {
        private readonly IDictionary<int, object> _cachedObjects = new Dictionary<int, object>();
        private readonly IDictionary<Type, object> _defaults = new Dictionary<Type, object>();
        private readonly IBuildSession _resolver;

        public SessionCache(IBuildSession resolver)
        {
            _resolver = resolver;
        }

        public SessionCache(IBuildSession resolver, ExplicitArguments arguments)
            : this(resolver)
        {
            _defaults = arguments.Defaults;
        }

        public object GetDefault(Type pluginType, IPipelineGraph pipelineGraph)
        {
            if (_defaults.ContainsKey(pluginType))
            {
                return _defaults[pluginType];
            }

            var instance = pipelineGraph.GetDefault(pluginType);
            var o = GetObject(pluginType, instance);

            if (!instance.IsUnique())
            {
                _defaults.Add(pluginType, o);
            }
            
            return o;
        }

        public object GetObject(Type pluginType, Instance instance)
        {
            if (instance.IsUnique())
            {
                return _resolver.BuildNewInSession(pluginType, instance);
            }

            int key = instance.InstanceKey(pluginType);
            if (!_cachedObjects.ContainsKey(key))
            {
                object o = _resolver.ResolveFromLifecycle(pluginType, instance);
                _cachedObjects[key] = o;

                return o;
            }

            return _cachedObjects[key];
        }
    }
}