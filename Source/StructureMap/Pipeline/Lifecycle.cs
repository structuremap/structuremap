using System;
using System.Collections.Generic;
using StructureMap.Attributes;
using StructureMap.Interceptors;
using StructureMap.Util;

namespace StructureMap.Pipeline
{
    public interface IObjectCache
    {
        object Locker { get; }

        int Count
        {
            get;
        }

        object Get(Type pluginType, Instance instance);
        void Set(Type pluginType, Instance instance, object value);
        void DisposeAndClear();
    }

    public class NulloObjectCache : IObjectCache
    {
        public object Locker
        {
            get { return new object(); }
        }

        public int Count
        {
            get { return 0; }
        }

        public object Get(Type pluginType, Instance instance)
        {
            return null;
        }

        public void Set(Type pluginType, Instance instance, object value)
        {
            // no-op
        }

        public void DisposeAndClear()
        {
            // no-op
        }
    }

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


    public interface ILifecycle
    {
        void EjectAll();
        IObjectCache FindCache();
    }

    public static class Lifecycles
    {
        public static ILifecycle GetLifecycle(InstanceScope scope)
        {
            switch (scope)
            {
                case InstanceScope.PerRequest:
                    return null;

                case InstanceScope.Singleton:
                    return new SingletonLifecycle();

                case InstanceScope.HttpContext:
                    return new HttpContextLifecycle();

                case InstanceScope.ThreadLocal:
                    return new ThreadLocalStorageLifecycle();

                case InstanceScope.Hybrid:
                    return new HybridLifecycle();

                case InstanceScope.HttpSession:
                    return new HttpSessionLifecycle();

                case InstanceScope.HybridHttpSession:
                    return new HybridSessionLifecycle();
            }
            
            throw new ArgumentOutOfRangeException("scope");
        }
    }

    public class ObjectBuilder
    {
        private readonly PipelineGraph _pipeline;
        private readonly InterceptorLibrary _library;
        private readonly IObjectCache _defaultCache;

        public ObjectBuilder(PipelineGraph pipeline, InterceptorLibrary library, IObjectCache defaultCache)
        {
            _pipeline = pipeline;
            _library = library;
            _defaultCache = defaultCache;
        }

        public object Resolve(Type pluginType, Instance instance, BuildSession session)
        {
            var cache = FindCache(pluginType, instance, session);
            lock (cache.Locker)
            {
                var returnValue = cache.Get(pluginType, instance);
                if (returnValue == null)
                {
                    returnValue = ConstructNew(pluginType, instance, session);
                    

                    cache.Set(pluginType, instance, returnValue);
                }

                return returnValue;
            }
        }

        public object ConstructNew(Type pluginType, Instance instance, BuildSession session)
        {
            object returnValue = instance.Build(pluginType, session);
            return ApplyInterception(pluginType, returnValue, session, instance);
        }

        public virtual object ApplyInterception(Type pluginType, object actualValue, BuildSession session, Instance instance)
        {
            if (actualValue == null) return null;

            try
            {
                return _library.FindInterceptor(actualValue.GetType()).Process(actualValue, session);
            }
            catch (Exception e)
            {
                throw new StructureMapException(308, e, instance.Name, actualValue.GetType());
            }

            
        }

        public IObjectCache FindCache(Type pluginType, Instance instance, BuildSession session)
        {
            var lifecycle = _pipeline.ForType(pluginType).Lifecycle;
            return lifecycle == null
                       ? _defaultCache
                       : lifecycle.FindCache();
        }
    }
}