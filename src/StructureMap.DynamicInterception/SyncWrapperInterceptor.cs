using Castle.DynamicProxy;
using System.Threading.Tasks;

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
            var methodInvocationResult = _interceptionBehavior.Intercept(methodInvocation);
            if (!methodInvocationResult.Successful)
            {
                throw methodInvocationResult.Exception;
            }

            if (ReflectionHelper.IsTask(methodInvocation.MethodInfo.ReturnType))
            {
                var actualType = methodInvocation.ActualReturnType;
                if (actualType == typeof(void))
                {
                    invocation.ReturnValue = Task.FromResult(true);
                }
                else
                {
                    var task = GetType().CallPrivateStaticGenericMethod("wrapInTask", actualType,
                        methodInvocationResult.ReturnValue);

                    invocation.ReturnValue = task;
                }
            }
            else
            {
                invocation.ReturnValue = methodInvocationResult.ReturnValue;
            }
        }

        private static object wrapInTask<T>(object value)
        {
            return Task.FromResult<T>((T)value);
        }
    }
}