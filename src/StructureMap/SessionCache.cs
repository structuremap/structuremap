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
        private readonly IInstanceResolver _resolver;

        public SessionCache(IInstanceResolver resolver)
        {
            _resolver = resolver;
        }

        public SessionCache(IInstanceResolver resolver, ExplicitArguments arguments)
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

            Instance instance = pipelineGraph.GetDefault(pluginType);
            object o = GetObject(pluginType, instance);

            _defaults.Add(pluginType, o);

            return o;
        }

        public object GetObject(Type pluginType, Instance instance)
        {
            int key = instance.InstanceKey(pluginType);
            if (!_cachedObjects.ContainsKey(key))
            {
                object o = _resolver.Resolve(pluginType, instance);
                _cachedObjects[key] = o;

                return o;
            }

            return _cachedObjects[key];
        }
    }
}