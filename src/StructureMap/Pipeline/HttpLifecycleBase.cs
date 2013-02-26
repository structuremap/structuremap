namespace StructureMap.Pipeline
{
    public abstract class HttpLifecycleBase<HTTP, NONHTTP> : LifecycleBase
        where HTTP : ILifecycle, new()
        where NONHTTP : ILifecycle, new()
    {
        private readonly ILifecycle _http;
        private readonly ILifecycle _nonHttp;

        public HttpLifecycleBase()
        {
            _http = new HTTP();
            _nonHttp = new NONHTTP();
        }

        public override void EjectAll(ILifecycleContext context)
        {
            if (HttpContextLifecycle.HasContext())
            {
                _http.EjectAll(context);
            }
            _nonHttp.EjectAll(context);
        }

        public override IObjectCache FindCache(ILifecycleContext context)
        {
            return HttpContextLifecycle.HasContext()
                       ? _http.FindCache(context)
                       : _nonHttp.FindCache(context);
        }
    }
}