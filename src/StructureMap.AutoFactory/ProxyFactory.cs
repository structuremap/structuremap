using Castle.DynamicProxy;

namespace StructureMap.AutoFactory
{
    public class ProxyFactory<TPluginType> where TPluginType : class
    {
        private readonly ProxyGenerator _proxyGenerator;
        private readonly IContext _context;

        public ProxyFactory(ProxyGenerator proxyGenerator, IContext context)
        {
            _proxyGenerator = proxyGenerator;
            _context = context;
        }

        public TPluginType Create()
        {
            var interceptor = new FactoryInterceptor(_context);

            return _proxyGenerator.CreateInterfaceProxyWithoutTarget<TPluginType>(interceptor);
        }
    }
}