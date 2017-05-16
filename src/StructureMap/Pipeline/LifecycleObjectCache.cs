using System;
using System.Collections.Concurrent;
using System.Linq;

namespace StructureMap.Pipeline
{
    public class LifecycleObjectCache : IObjectCache
    {
        private readonly ConcurrentDictionary<int, object> _objects = new ConcurrentDictionary<int, object>();

        public int Count => _objects.Count;

        public bool Has(Type pluginType, Instance instance)
        {
            var key = instance.InstanceKey(pluginType);
            object @object;
            return _objects.TryGetValue(key, out @object);
        }

        public void Eject(Type pluginType, Instance instance)
        {
            // Prevent null reference exception.
            if (pluginType.AssemblyQualifiedName == null)
                return;

            var key = instance.InstanceKey(pluginType);

            object @object;
            if (_objects.TryRemove(key, out @object))
            {
                @object.SafeDispose();
            }
        }

        public object Get(Type pluginType, Instance instance, IBuildSession session)
        {
            var key = instance.InstanceKey(pluginType);

            return _objects.GetOrAdd(key, _ => buildWithSession(pluginType, instance, session));
        }

        protected virtual object buildWithSession(Type pluginType, Instance instance, IBuildSession session)
        {
            return session.BuildNewInOriginalContext(pluginType, instance);
        }

        public void DisposeAndClear()
        {
            var keys = _objects.Keys.ToList();

            foreach (var key in keys)
            {
                object @object;
                if (_objects.TryRemove(key, out @object))
                {
                    if (@object != null && !(@object is Container))
                    {
                        @object.SafeDispose();
                    }
                }
            }
        }

        public void Set(Type pluginType, Instance instance, object value)
        {
            if (value == null) return;

            var key = instance.InstanceKey(pluginType);
            _objects.AddOrUpdate(key, value, (_, __) => value);
        }
    }
}