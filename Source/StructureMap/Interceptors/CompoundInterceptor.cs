namespace StructureMap.Interceptors
{
    public class CompoundInterceptor : InstanceInterceptor
    {
        private readonly InstanceInterceptor[] _interceptors;

        public CompoundInterceptor(InstanceInterceptor[] interceptors)
        {
            _interceptors = interceptors;
        }


        public InstanceInterceptor[] Interceptors
        {
            get { return _interceptors; }
        }

        #region InstanceInterceptor Members

        public object Process(object target)
        {
            object returnValue = target;
            foreach (InstanceInterceptor interceptor in _interceptors)
            {
                returnValue = interceptor.Process(returnValue);
            }

            return returnValue;
        }

        #endregion
    }
}