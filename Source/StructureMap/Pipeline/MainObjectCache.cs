using System;
using System.Collections.Generic;

namespace StructureMap.Pipeline
{
    public class MainObjectCache : IObjectCache
    {
        private readonly object _locker = new object();
        private readonly IDictionary<InstanceKey, object> _objects = new Dictionary<InstanceKey, object>();

        public object Locker { get { return _locker; } }

        public int Count { get { return _objects.Count; } }

        public object Get(Type pluginType, Instance instance)
        {
            var key = new InstanceKey(instance, pluginType);
            return _objects.ContainsKey(key) ? _objects[key] : null;
        }

        public void Set(Type pluginType, Instance instance, object value)
        {
            if (value == null) return;

            try
            {
                var key = new InstanceKey(instance, pluginType);
                _objects.Add(key, value);
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
                foreach (object @object in _objects.Values)
                {
                    if (@object is Container) continue;

                    var disposable = @object as IDisposable;
                    if (disposable != null)
                    {
                        try
                        {
                            disposable.Dispose();
                        }
                        catch (Exception)
                        {
                        }
                    }
                }

                _objects.Clear();
            }
        }
    }
}