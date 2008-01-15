namespace StructureMap.Interceptors
{
    public class StartupInterceptor<T> : InstanceInterceptor
    {
        private readonly StartupHandler<T> _handler;

        public StartupInterceptor(StartupHandler<T> handler)
        {
            _handler = handler;
        }

        #region InstanceInterceptor Members

        public object Process(object target)
        {
            _handler((T) target);
            return target;
        }

        #endregion
    }
}