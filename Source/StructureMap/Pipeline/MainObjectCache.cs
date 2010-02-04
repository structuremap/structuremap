using System;
using StructureMap.Util;

namespace StructureMap.Pipeline
{
    public class MainObjectCache : IObjectCache
    {
        private readonly object _locker = new object();
        private readonly Cache<InstanceKey, object> _objects = new Cache<InstanceKey, object>();

        public object Locker { get { return _locker; } }

        public int Count { get { return _objects.Count; } }

        public bool Has(Type pluginType, Instance instance)
        {
            var key = new InstanceKey(instance, pluginType);
            return _objects.Has(key);
        }

        public void Eject(Type pluginType, Instance instance)
        {
            var key = new InstanceKey(instance, pluginType);
            if (!_objects.Has(key)) return;

            var disposable = _objects[key] as IDisposable;
            _objects.Remove(key);
            disposable.SafeDispose();
        }

        public object Get(Type pluginType, Instance instance)
        {
            var key = new InstanceKey(instance, pluginType);
            return _objects.Has(key) ? _objects[key] : null;
        }

        public void Set(Type pluginType, Instance instance, object value)
        {
            if (value == null) return;

            try
            {
                var key = new InstanceKey(instance, pluginType);
                _objects[key] = value;
            }
            catch (ArgumentException e)
            {
                string message = string.Format("Duplicate key for Instance {0} of PluginType {1}", instance.Name,
                                               pluginType.AssemblyQualifiedName);
                throw new ArgumentException(message, e);
            }
        }

        public void DisposeAndClear()
        {
            lock (Locker)
            {
                _objects.Each(@object =>
                {
                    if (@object is Container) return;

                    @object.SafeDispose();
                });

                _objects.Clear();
            }
        }

    }
}