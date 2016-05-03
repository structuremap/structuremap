using Castle.DynamicProxy;

namespace StructureMap.DynamicInterception
{
    internal class AsyncWrapperInterceptor : IInterceptor
    {
        private readonly IAsyncInterceptionBehavior _interceptionBehavior;

        public AsyncWrapperInterceptor(IAsyncInterceptionBehavior interceptionBehavior)
        {
            _interceptionBehavior = interceptionBehavior;
        }

        public void Intercept(IInvocation invocation)
        {
            var methodInvocation = new MethodInvocation(invocation);
            var methodInvocationResultTask = _interceptionBehavior.InterceptAsync(methodInvocation);

            if (!ReflectionHelper.IsTask(methodInvocation.MethodInfo.ReturnType))
            {
                var methodInvocationResult = ReflectionHelper.GetTaskResult(methodInvocationResultTask);

                invocation.ReturnValue = methodInvocationResult.GetReturnValueOrThrow();
            }
            else
            {
                var actualReturnType = methodInvocation.ActualReturnType;
                invocation.ReturnValue = actualReturnType == typeof(void)
                    ? ReflectionHelper.ConvertInvocationResultToTask(methodInvocationResultTask)
                    : ReflectionHelper.ConvertInvocationResultToTask(actualReturnType,
                        methodInvocationResultTask);
            }
        }
    }
}