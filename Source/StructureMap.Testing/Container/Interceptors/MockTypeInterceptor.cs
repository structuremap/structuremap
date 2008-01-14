using System;
using System.Collections.Generic;
using StructureMap.Interceptors;

namespace StructureMap.Testing.Container.Interceptors
{
    

    public class MockTypeInterceptor : TypeInterceptor
    {
        private readonly List<Type> _types = new List<Type>();
        private Dictionary<Type, InstanceInterceptor> _innerInterceptors = new Dictionary<Type, InstanceInterceptor>();

        public MockTypeInterceptor(params Type[] types)
        {
            _types.AddRange(types);
        }

        public void SetToMatch<T>()
        {
            _types.Add(typeof(T));
        }

        public bool MatchesType(Type type)
        {
            return _types.Contains(type);
        }

        public object Process(object target)
        {
            return _innerInterceptors[target.GetType()].Process(target);            
        }

        public void AddHandler<T>(InterceptionDelegate<T> handler)
        {
            _types.Add(typeof(T));
            _innerInterceptors.Add(typeof(T), new CommonInterceptor<T>(handler));
        }

        public delegate object InterceptionDelegate<T>(T target);
        public class CommonInterceptor<T> : InstanceInterceptor
        {
            private readonly InterceptionDelegate<T> _handler;

            public CommonInterceptor(InterceptionDelegate<T> handler)
            {
                _handler = handler;
            }

            public object Process(object target)
            {
                return _handler((T) target);
            }
        }
    }
}