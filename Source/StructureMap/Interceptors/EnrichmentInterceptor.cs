namespace StructureMap.Interceptors
{
    // TODO -- gotta change to use IContext
    public class EnrichmentInterceptor<T> : InstanceInterceptor
    {
        private readonly ContextEnrichmentHandler<T> _handler;


        public EnrichmentInterceptor(ContextEnrichmentHandler<T> handler)
        {
            _handler = handler;
        }

        #region InstanceInterceptor Members

        public object Process(object target, IContext context)
        {
            return _handler(context, (T) target);
        }

        #endregion
    }
}