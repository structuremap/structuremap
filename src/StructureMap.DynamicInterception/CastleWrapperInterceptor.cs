using Castle.DynamicProxy;

namespace StructureMap.DynamicInterception
{
    internal class CastleWrapperInterceptor : IInterceptor
    {
        private readonly IInterceptionBehavior _interceptionBehavior;

        public CastleWrapperInterceptor(IInterceptionBehavior interceptionBehavior)
        {
            _interceptionBehavior = interceptionBehavior;
        }

        public void Intercept(IInvocation invocation)
        {
            var methodInvocation = new MethodInvocation(invocation);
            var methodInvocationResult = _interceptionBehavior.Intercept(methodInvocation);
            if (!methodInvocationResult.Successful)
            {
                throw methodInvocationResult.Exception;
            }

            invocation.ReturnValue = methodInvocationResult.ReturnValue;
        }
    }
}