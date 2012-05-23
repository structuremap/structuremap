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

        	int remainingArguments = invocation.Arguments.Length;
            if ((invocation.Arguments.Length > 0) && (invocation.Arguments[0] is Type))
            {
                pluginType = (Type) invocation.Arguments[0];
            	remainingArguments--;
            }
            else
            {
                pluginType = invocation.Method.ReturnType;                
            }

        	var returnValue = GetReturnValue(invocation, pluginType, remainingArguments);
        	invocation.ReturnValue = returnValue;
        }

    	private object GetReturnValue(IInvocation invocation, Type pluginType, int remainingArguments)
    	{
    		object returnValue;
    		if (remainingArguments > 0)
    		{
    			var container = _context.GetInstance<IContainer>();
    			var i = invocation.Arguments.Length - remainingArguments;
    			returnValue = GetReturnValueForMultipleArguments(invocation, pluginType, i, container);
    		}
    		else
    			returnValue = _context.GetInstance(pluginType);
    		return returnValue;
    	}

    	private static object GetReturnValueForMultipleArguments(IInvocation invocation, Type pluginType, int i,
    	                                                         IContainer container)
    	{
    		var parameters = invocation.Method.GetParameters();
    		var name = parameters[i].Name;
    		var value = invocation.Arguments[i];
    		return container.With(name).EqualTo(value).GetInstance(pluginType);
    	}
    }
}