using Castle.DynamicProxy;

namespace StructureMap.DynamicInterception
{
    internal class CastleInterceptor : IInterceptor
    {
        private readonly IInterceptionBehavior[] _interceptionBehaviors;

        public CastleInterceptor(IInterceptionBehavior[] interceptionBehaviors)
        {
            _interceptionBehaviors = interceptionBehaviors;
        }

        public void Intercept(IInvocation invocation)
        {
            var methodInvocation = new MethodInvocation(invocation, _interceptionBehaviors);

            var methodInvocationResultTask = methodInvocation.InvokeNextAsync();

            var returnType = methodInvocation.MethodInfo.ReturnType;
            if (ReflectionHelper.IsTask(returnType))
            {
                invocation.ReturnValue = ReflectionHelper.IsNonGenericTask(returnType)
                    ? ReflectionHelper.ConvertInvocationResultToTask(methodInvocationResultTask)
                    : ReflectionHelper.ConvertInvocationResultToTask(methodInvocation.ActualReturnType,
                        methodInvocationResultTask);
            }
            else
            {
                var methodInvocationResult = ReflectionHelper.GetResultFromTask(methodInvocationResultTask);
                invocation.ReturnValue = methodInvocationResult.GetReturnValueOrThrow();
            }
        }
    }
}