namespace StructureMap.Interceptors
{
    public class EnrichmentInterceptor<T> : InstanceInterceptor
    {
        private readonly ContextEnrichmentHandler<T> _handler;

        public EnrichmentInterceptor(ContextEnrichmentHandler<T> handler)
        {
            _handler = handler;
        }

        public object Process(object target, IContext context)
        {
            return _handler(context, (T) target);
        }
    }
}