using System;

namespace StructureMap.Interceptors
{
    [Obsolete]
    public class MatchedTypeInterceptor : TypeInterceptor
    {
        private readonly Predicate<Type> _match;
        private Func<IContext, object, object> _interception;

        public MatchedTypeInterceptor(Predicate<Type> match)
        {
            _match = match;
        }

        #region TypeInterceptor Members

        public bool MatchesType(Type type)
        {
            return _match(type);
        }

        public object Process(object target, IBuildSession session)
        {
            return _interception(session, target);
        }

        #endregion

        /// <summary>
        /// Specify how objects matching the Type predicate
        /// will be intercepted
        /// </summary>
        /// <param name="interception"></param>
        public void InterceptWith(Func<object, object> interception)
        {
            _interception = (context, o) => interception(o);
        }

        /// <summary>
        /// Specify how objects matching the Type predicate
        /// will be intercepted
        /// </summary>
        /// <param name="interception"></param>
        public void InterceptWith(Func<IContext, object, object> interception)
        {
            _interception = interception;
        }
    }
}