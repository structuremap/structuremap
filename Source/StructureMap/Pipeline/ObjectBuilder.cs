using System;
using StructureMap.Interceptors;

namespace StructureMap.Pipeline
{
    public class ObjectBuilder
    {
        private readonly IObjectCache _defaultCache;
        private readonly InterceptorLibrary _library;
        private readonly PipelineGraph _pipeline;

        public ObjectBuilder(PipelineGraph pipeline, InterceptorLibrary library, IObjectCache defaultCache)
        {
            if (pipeline == null) throw new ArgumentNullException("pipeline");

            _pipeline = pipeline;
            _library = library;
            _defaultCache = defaultCache;
        }

        public object Resolve(Type pluginType, Instance instance, BuildSession session)
        {
            IObjectCache cache = FindCache(pluginType, instance, session);
            lock (cache.Locker)
            {
                object returnValue = cache.Get(pluginType, instance);
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

        public virtual object ApplyInterception(Type pluginType, object actualValue, BuildSession session,
                                                Instance instance)
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
            ILifecycle lifecycle = _pipeline.ForType(pluginType).Lifecycle;
            return lifecycle == null
                       ? _defaultCache
                       : lifecycle.FindCache();
        }
    }
}