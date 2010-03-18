using System;
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
            Type pluginType;

            if ((invocation.Arguments.Length > 0) && (invocation.Arguments[0] is Type))
            {
                pluginType = (Type) invocation.Arguments[0];
            }
            else
            {
                pluginType = invocation.Method.ReturnType;                
            }

            invocation.ReturnValue = _context.GetInstance(pluginType);
        }
    }
}