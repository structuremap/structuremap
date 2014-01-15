using System;
using System.Collections.Generic;
using StructureMap.Interceptors;

namespace StructureMap.Testing.Graph.Interceptors
{
    public class MockTypeInterceptor : TypeInterceptor
    {
        #region Delegates

        public delegate object InterceptionDelegate<T>(T target);

        #endregion

        private readonly Dictionary<Type, InstanceInterceptor> _innerInterceptors
            = new Dictionary<Type, InstanceInterceptor>();

        private readonly List<Type> _types = new List<Type>();

        public MockTypeInterceptor(params Type[] types)
        {
            _types.AddRange(types);
        }

        #region TypeInterceptor Members

        public bool MatchesType(Type type)
        {
            return _types.Contains(type);
        }

        public object Process(object target, IBuildSession session)
        {
            return _innerInterceptors[target.GetType()].Process(target, session);
        }

        #endregion

        public void SetToMatch<T>()
        {
            _types.Add(typeof (T));
        }

        public void AddHandler<T>(InterceptionDelegate<T> handler)
        {
            _types.Add(typeof (T));
            _innerInterceptors.Add(typeof (T), new CommonInterceptor<T>(handler));
        }

        #region Nested type: CommonInterceptor

        public class CommonInterceptor<T> : InstanceInterceptor
        {
            private readonly InterceptionDelegate<T> _handler;

            public CommonInterceptor(InterceptionDelegate<T> handler)
            {
                _handler = handler;
            }

            #region InstanceInterceptor Members

            public object Process(object target, IBuildSession session)
            {
                return _handler((T) target);
            }

            #endregion
        }

        #endregion
    }
}