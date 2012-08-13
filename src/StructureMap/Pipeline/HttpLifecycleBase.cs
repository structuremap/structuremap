namespace StructureMap.Pipeline
{
    public abstract class HttpLifecycleBase<HTTP, NONHTTP> : ILifecycle
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

        public void EjectAll()
        {
            if (HttpContextLifecycle.HasContext())
            {
                _http.EjectAll();
            }
            _nonHttp.EjectAll();
        }

        public IObjectCache FindCache()
        {
            return HttpContextLifecycle.HasContext()
                       ? _http.FindCache()
                       : _nonHttp.FindCache();
        }

        public abstract string Scope { get; }
    }
}