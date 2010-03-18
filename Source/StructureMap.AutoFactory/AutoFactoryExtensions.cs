using System;
using Castle.Core.Interceptor;
using Castle.DynamicProxy;
using StructureMap.Configuration.DSL.Expressions;

namespace StructureMap.AutoFactory
{
    public static class AutoFactoryExtensions
    {
        private static readonly ProxyGenerator proxyGenerator = new ProxyGenerator();

        public static void CreateFactory<PLUGINTYPE>(this CreatePluginFamilyExpression<PLUGINTYPE> expression) 
            where PLUGINTYPE : class
        {
            var callback = CreateFactoryCallback<PLUGINTYPE>();

            expression.Use(callback);
        }

        private static Func<IContext, PLUGINTYPE> CreateFactoryCallback<PLUGINTYPE>() 
            where PLUGINTYPE : class
        {
            return ctxt => 
            {
                var proxyFactory = new ProxyFactory<PLUGINTYPE>(proxyGenerator, ctxt);

                return proxyFactory.Create();
            };
        }
    }

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

    public class FactoryInterceptor : IInterceptor
    {
        private readonly IContext _context;

        public FactoryInterceptor(IContext context)
        {
            _context = context;
        }

        public void Intercept(IInvocation invocation)
        {
            var method = invocation.Method;
            var pluginType = method.ReturnType;

            invocation.ReturnValue = _context.GetInstance(pluginType);
        }
    }
}