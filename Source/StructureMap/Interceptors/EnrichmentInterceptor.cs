namespace StructureMap.Interceptors
{
    public class EnrichmentInterceptor<T> : InstanceInterceptor
    {
        private readonly EnrichmentHandler<T> _handler;


        public EnrichmentInterceptor(EnrichmentHandler<T> handler)
        {
            _handler = handler;
        }

        #region InstanceInterceptor Members

        public object Process(object target)
        {
            return _handler((T) target);
        }

        #endregion
    }
}