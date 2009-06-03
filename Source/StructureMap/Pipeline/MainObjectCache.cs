using System;
using System.Collections.Generic;

namespace StructureMap.Pipeline
{
    public class MainObjectCache : IObjectCache
    {
        private readonly IDictionary<InstanceKey, object> _objects = new Dictionary<InstanceKey,object>();
        private readonly object _locker = new object();

        public object Locker
        {
            get { return _locker; }
        }

        public int Count
        {
            get { return _objects.Count; }
        }

        public object Get(Type pluginType, Instance instance)
        {
            var key = new InstanceKey(instance, pluginType);
            return _objects.ContainsKey(key) ? _objects[key] : null;
        }

        public void Set(Type pluginType, Instance instance, object value)
        {
            var key = new InstanceKey(instance, pluginType);
            _objects.Add(key, value);
        }

        public void DisposeAndClear()
        {
            lock (Locker)
            {
                foreach (var @object in _objects.Values)
                {
                    IDisposable disposable = @object as IDisposable;
                    if (disposable != null)
                    {
                        try
                        {
                            disposable.Dispose();
                        }
                        catch (Exception) { }
                    }
                }
            
                _objects.Clear();
            }
        }
    }
}