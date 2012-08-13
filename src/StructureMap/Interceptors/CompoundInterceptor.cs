namespace StructureMap.Interceptors
{
    public class CompoundInterceptor : InstanceInterceptor
    {
        private readonly InstanceInterceptor[] _interceptors;

        public CompoundInterceptor(InstanceInterceptor[] interceptors)
        {
            _interceptors = interceptors;
        }


        public InstanceInterceptor[] Interceptors { get { return _interceptors; } }

        #region InstanceInterceptor Members

        public object Process(object target, IContext context)
        {
            object returnValue = target;
            foreach (InstanceInterceptor interceptor in _interceptors)
            {
                returnValue = interceptor.Process(returnValue, context);
            }

            return returnValue;
        }

        #endregion

        public InstanceInterceptor Merge(InstanceInterceptor interceptor)
        {
            var interceptors = new InstanceInterceptor[_interceptors.Length + 1];
            _interceptors.CopyTo(interceptors, 0);
            interceptors[interceptors.Length - 1] = interceptor;

            return new CompoundInterceptor(interceptors);
        }
    }
}