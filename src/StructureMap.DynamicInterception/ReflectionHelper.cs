using System;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace StructureMap.DynamicInterception
{
    internal static class ReflectionHelper
    {
        private static object callPrivateStaticGenericMethod(this Type methodHolderType, string methodName,
            Type genericType, object parameter)
        {
            try
            {
                return methodHolderType
                    .GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic)
                    .MakeGenericMethod(genericType)
                    .Invoke(null, new[] { parameter });
            }
            catch (TargetInvocationException e)
            {
                rethrowInnerException(e);
                return null;
            }
        }

        private static void rethrowInnerException(Exception e)
        {
            ExceptionDispatchInfo.Capture(e.InnerException ?? e).Throw();
        }

        public static Type GetActualType(Type type)
        {
            if (IsNonGenericTask(type))
            {
                return typeof(void);
            }

            if (IsGenericTask(type))
            {
                return GetTypeFromGenericTask(type);
            }

            return type;
        }

        public static Type GetTypeFromGenericTask(Type type)
        {
            return type.GetGenericArguments()[0];
        }

        public static bool IsTask(Type type)
        {
            return typeof(Task).IsAssignableFrom(type);
        }

        public static bool IsNonGenericTask(Type type)
        {
            return type == typeof(Task);
        }

        public static bool IsGenericTask(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>);
        }

        public static async Task ConvertInvocationResultToTask(IMethodInvocationResult methodInvocationResult)
        {
            methodInvocationResult.GetReturnValueOrThrow();
        }

        public static object ConvertInvocationResultToTask(Type resultType, IMethodInvocationResult methodInvocationResult)
        {
            return callPrivateStaticGenericMethod(typeof(ReflectionHelper), "convertInvocationResultToGenericTask", resultType,
                methodInvocationResult);
        }

        private static async Task<T> convertInvocationResultToGenericTask<T>(IMethodInvocationResult methodInvocationResult)
        {
            return (T)methodInvocationResult.GetReturnValueOrThrow();
        }

        public static async Task ConvertInvocationResultToTask(Task<IMethodInvocationResult> methodInvocationResultTask)
        {
            var methodInvocationResult = await methodInvocationResultTask.ConfigureAwait(false);
            methodInvocationResult.GetReturnValueOrThrow();
        }

        public static object ConvertInvocationResultToTask(Type resultType, Task<IMethodInvocationResult> methodInvocationResult)
        {
            return callPrivateStaticGenericMethod(typeof(ReflectionHelper), "convertInvocationResultTaskToGenericTask", resultType,
                methodInvocationResult);
        }

        private static async Task<T> convertInvocationResultTaskToGenericTask<T>(Task<IMethodInvocationResult> methodInvocationResultTask)
        {
            var methodInvocationResult = await methodInvocationResultTask.ConfigureAwait(false);

            return (T)methodInvocationResult.GetReturnValueOrThrow();
        }

        public static object GetTaskResult(Type resultType, object task)
        {
            return callPrivateStaticGenericMethod(typeof(ReflectionHelper), "getTaskResult", resultType,
                task);
        }

        public static T GetTaskResult<T>(Task<T> task)
        {
            return getTaskResult(task);
        }

        private static T getTaskResult<T>(Task<T> task)
        {
            try
            {
                return task.Result;
            }
            catch (AggregateException e)
            {
                rethrowInnerException(e);
                return default(T);
            }
        }

        public static Task<object> GetTaskResultAsync(Type resultType, object task)
        {
            return (Task<object>)callPrivateStaticGenericMethod(typeof(ReflectionHelper),
                "getTaskResultAsync",
                resultType,
                task);
        }

        private static async Task<object> getTaskResultAsync<T>(object task)
        {
            return await ((Task<T>)task).ConfigureAwait(false);
        }
    }
}