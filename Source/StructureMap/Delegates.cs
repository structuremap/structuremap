namespace StructureMap
{
    public delegate T EnrichmentHandler<T>(T target);
    public delegate void StartupHandler<T>(T target);

    public interface InstanceInterceptor
    {
        object Process(object target);
    }

    public class NulloInterceptor : InstanceInterceptor
    {
        public object Process(object target)
        {
            return target;
        }
    }

    public class StartupInterceptor<T> : InstanceInterceptor
    {
        private readonly StartupHandler<T> _handler;

        public StartupInterceptor(StartupHandler<T> handler)
        {
            _handler = handler;
        }


        public object Process(object target)
        {
            _handler((T) target);
            return target;
        }
    }

    public class EnrichmentInterceptor<T> : InstanceInterceptor
    {
        private readonly EnrichmentHandler<T> _handler;


        public EnrichmentInterceptor(EnrichmentHandler<T> handler)
        {
            _handler = handler;
        }

        public object Process(object target)
        {
            return _handler((T) target);
        }
    }
}
