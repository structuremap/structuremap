using Castle.DynamicProxy;

namespace StructureMap.AutoFactory
{
    public class ProxyFactory<PLUGINTYPE> where PLUGINTYPE : class
    {
        private readonly ProxyGenerator _proxyGenerator;
        private readonly IContext _context;

        public ProxyFactory(ProxyGenerator proxyGenerator, IContext context)
        {
            _proxyGenerator = proxyGenerator;
            _context = context;
        }

        public PLUGINTYPE Create()
        {
            var interceptor = new FactoryInterceptor(_context);

            return _proxyGenerator.CreateInterfaceProxyWithoutTarget<PLUGINTYPE>(interceptor);
        }
    }
}