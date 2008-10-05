using System;

namespace StructureMap.Interceptors
{
    public class MatchedTypeInterceptor : TypeInterceptor
    {
        private readonly Predicate<Type> _match;
        private Func<object, object> _interception;

        internal MatchedTypeInterceptor(Predicate<Type> match)
        {
            _match = match;
        }

        #region TypeInterceptor Members

        public bool MatchesType(Type type)
        {
            return _match(type);
        }

        public object Process(object target)
        {
            return _interception(target);
        }

        #endregion

        public void InterceptWith(Func<object, object> interception)
        {
            _interception = interception;
        }
    }
}