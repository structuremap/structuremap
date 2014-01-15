using System;

namespace StructureMap.Interceptors
{
    [Obsolete]
    public class NulloInterceptor : InstanceInterceptor
    {
        #region InstanceInterceptor Members

        public object Process(object target, IBuildSession session)
        {
            return target;
        }

        #endregion
    }
}