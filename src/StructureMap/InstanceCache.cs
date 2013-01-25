using System;
using System.Collections.Generic;
using StructureMap.Pipeline;

namespace StructureMap
{
    public class InstanceCache
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
            if (pluginType == null)
            {
                throw new ArgumentNullException("pluginType");
            }

            if (instance == null)
            {
                throw new ArgumentNullException("instance",
                                                "Trying to find an Instance of type " + pluginType.AssemblyQualifiedName);
            }

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
            if (result == null) return;

            Dictionary<Instance, object> cache = getCache(pluginType);

            if (cache.ContainsKey(Instance))
            {
                string message = string.Format("Duplicate Objects detected for Instance {0} of Type {1}", Instance.Name,
                                               pluginType.AssemblyQualifiedName);
                throw new ApplicationException(message);
            }
            cache.Add(Instance, result);
        }

        public void Each<T>(Action<T> action) where T : class
        {
            foreach (var dictionary in _objects.Values)
            {
                foreach (object o in dictionary.Values)
                {
                    var t = o as T;
                    if (t != null)
                    {
                        action(t);
                    }
                }
            }
        }
    }
}