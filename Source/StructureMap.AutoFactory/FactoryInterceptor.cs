using Castle.Core.Interceptor;

namespace StructureMap.AutoFactory
{
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