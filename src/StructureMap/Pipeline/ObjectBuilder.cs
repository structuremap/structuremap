using System;
using StructureMap.Interceptors;

namespace StructureMap.Pipeline
{
    public class ObjectBuilder : IObjectBuilder
    {
        private readonly InterceptorLibrary _library;

        public ObjectBuilder(InterceptorLibrary library)
        {
            _library = library;
        }

        public object Resolve(Type pluginType, Instance instance, BuildSession session, IPipelineGraph pipeline)
        {
            IObjectCache cache = pipeline.FindCache(pluginType);
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
    }
}