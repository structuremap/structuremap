using System;
using System.Collections.Generic;
using StructureMap.Pipeline;

namespace StructureMap
{
    internal class InstanceCache
    {
        private readonly Dictionary<Type, Dictionary<Instance, object>> _objects =
            new Dictionary<Type, Dictionary<Instance, object>>();

        private Dictionary<Instance, object> getCache(Type type)
        {
            if (_objects.ContainsKey(type))
            {
                return _objects[type];
            }
            else
            {
                var cache = new Dictionary<Instance, object>();
                _objects.Add(type, cache);
                return cache;
            }
        }

        public object Get(Type pluginType, Instance instance)
        {
            Dictionary<Instance, object> cache = getCache(pluginType);
            if (cache.ContainsKey(instance))
            {
                return cache[instance];
            }
            else
            {
                return null;
            }
        }

        public void Set(Type pluginType, Instance Instance, object result)
        {
            Dictionary<Instance, object> cache = getCache(pluginType);
            cache.Add(Instance, result);
        }
    }
}