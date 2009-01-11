namespace StructureMap.Interceptors
{
    public class NulloInterceptor : InstanceInterceptor
    {
        #region InstanceInterceptor Members

        public object Process(object target, IContext context)
        {
            return target;
        }

        #endregion
    }
}