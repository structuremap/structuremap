using System.Collections;
using System.Collections.Generic;
using StructureMap.Interceptors;

namespace StructureMap.Graph
{
    /// <summary>
    /// Manages a list of InstanceFactoryInterceptor's.  Design-time model of an array
    /// of decorators to alter the InstanceFactory behavior for a PluginType.
    /// </summary>
    public class InterceptionChain : IEnumerable<InstanceFactoryInterceptor>
    {
        private List<InstanceFactoryInterceptor> _interceptorList;

        public InterceptionChain()
        {
            _interceptorList = new List<InstanceFactoryInterceptor>();
        }

        public IInstanceFactory WrapInstanceFactory(IInstanceFactory factory)
        {
            IInstanceFactory outerFactory = factory;

            for (int i = _interceptorList.Count - 1; i >= 0; i--)
            {
                InstanceFactoryInterceptor interceptor = _interceptorList[i];
                interceptor.InnerInstanceFactory = outerFactory;
                outerFactory = interceptor;
            }

            return outerFactory;
        }

        public void AddInterceptor(InstanceFactoryInterceptor interceptor)
        {
            _interceptorList.Add(interceptor);
        }

        public int Count
        {
            get { return _interceptorList.Count; }
        }

        public InstanceFactoryInterceptor this[int index]
        {
            get { return _interceptorList[index]; }
        }

        IEnumerator<InstanceFactoryInterceptor> IEnumerable<InstanceFactoryInterceptor>.GetEnumerator()
        {
            return _interceptorList.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return _interceptorList.GetEnumerator();
        }
    }
}