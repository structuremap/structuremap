using Castle.DynamicProxy;
using System.Threading.Tasks;

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
                var methodInvocationResult = methodInvocationResultTask.Result;
                if (!methodInvocationResult.Successful)
                {
                    throw methodInvocationResult.Exception;
                }

                invocation.ReturnValue = methodInvocationResult.ReturnValue;
            }
            else
            {
                var actualReturnType = methodInvocation.ActualReturnType;
                if (actualReturnType == typeof(void))
                {
                    invocation.ReturnValue = interceptNonGeneric(methodInvocationResultTask);
                }
                else
                {
                    invocation.ReturnValue = GetType().CallPrivateStaticGenericMethod("interceptGeneric",
                        actualReturnType, methodInvocationResultTask);
                }
            }
        }

        private static async Task interceptNonGeneric(Task<IMethodInvocationResult> methodInvocationResultTask)
        {
            var methodInvocationResult = await methodInvocationResultTask.ConfigureAwait(false);
            if (!methodInvocationResult.Successful)
            {
                throw methodInvocationResult.Exception;
            }
        }

        private static async Task<T> interceptGeneric<T>(Task<IMethodInvocationResult> methodInvocationResultTask)
        {
            var methodInvocationResult = await methodInvocationResultTask.ConfigureAwait(false);
            if (!methodInvocationResult.Successful)
            {
                throw methodInvocationResult.Exception;
            }

            return (T)methodInvocationResult.ReturnValue;
        }
    }
}