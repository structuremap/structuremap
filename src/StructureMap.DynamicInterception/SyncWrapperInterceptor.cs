using Castle.DynamicProxy;
using System;

namespace StructureMap.DynamicInterception
{
    internal class SyncWrapperInterceptor : IInterceptor
    {
        private readonly ISyncInterceptionBehavior _interceptionBehavior;

        public SyncWrapperInterceptor(ISyncInterceptionBehavior interceptionBehavior)
        {
            _interceptionBehavior = interceptionBehavior;
        }

        public void Intercept(IInvocation invocation)
        {
            var methodInvocation = new MethodInvocation(invocation);
            IMethodInvocationResult methodInvocationResult;
            try
            {
                methodInvocationResult = _interceptionBehavior.Intercept(methodInvocation);
            }
            catch (Exception e)
            {
                methodInvocationResult = methodInvocation.CreateExceptionResult(e);
            }

            if (ReflectionHelper.IsTask(methodInvocation.MethodInfo.ReturnType))
            {
                var actualType = methodInvocation.ActualReturnType;

                invocation.ReturnValue = actualType == typeof(void)
                    ? ReflectionHelper.ConvertInvocationResultToTask(methodInvocationResult)
                    : ReflectionHelper.ConvertInvocationResultToTask(actualType, methodInvocationResult);
            }
            else
            {
                invocation.ReturnValue = methodInvocationResult.GetReturnValueOrThrow();
            }
        }
    }
}