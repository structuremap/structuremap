using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace StructureMap.DynamicInterception
{
    internal class MethodInvocation : ISyncMethodInvocation, IAsyncMethodInvocation
    {
        private readonly IInvocation _invocation;
        private readonly IList<IArgument> _arguments;
        private readonly IDictionary<string, IArgument> _argumentMap;

        public MethodInvocation(IInvocation invocation)
        {
            _invocation = invocation;
            _arguments = invocation.Method.GetParameters()
                .Zip(invocation.Arguments, (info, value) => new { info, value })
                .Select((t, i) => new Argument(invocation, i, t.value, t.info))
                .ToList<IArgument>();
            _argumentMap = _arguments.ToDictionary(a => a.ParameterInfo.Name);
        }

        public IList<IArgument> Arguments
        {
            get { return new List<IArgument>(_arguments); }
        }

        public IArgument GetArgument(string name)
        {
            IArgument result;
            return _argumentMap.TryGetValue(name, out result) ? result : null;
        }

        public MethodInfo MethodInfo
        {
            get { return _invocation.Method; }
        }

        public IMethodInvocationResult InvokeNext()
        {
            try
            {
                _invocation.Proceed();

                if (_invocation.ReturnValue == null)
                {
                    return CreateResult(null);
                }

                var returnType = _invocation.ReturnValue.GetType();

                if (ReflectionHelper.IsNonGenericTask(returnType))
                {
                    return CreateResult(null);
                }

                if (ReflectionHelper.IsGenericTask(returnType))
                {
                    var result = GetType().CallPrivateStaticGenericMethod("getTaskResult", ReflectionHelper.GetTypeFromGenericTask(returnType), _invocation.ReturnValue);

                    return CreateResult(result);
                }

                return CreateResult(_invocation.ReturnValue);
            }
            catch (Exception e)
            {
                return CreateExceptionResult(e);
            }
        }

        private static T getTaskResult<T>(Task<T> task)
        {
            return task.Result;
        }

        public async Task<IMethodInvocationResult> InvokeNextAsync()
        {
            try
            {
                _invocation.Proceed();

                if (_invocation.ReturnValue == null)
                {
                    return CreateResult(null);
                }

                var returnType = _invocation.ReturnValue.GetType();

                if (ReflectionHelper.IsNonGenericTask(returnType))
                {
                    await ((Task)_invocation.ReturnValue).ConfigureAwait(false);
                    return CreateResult(true);
                }

                if (ReflectionHelper.IsGenericTask(returnType))
                {
                    var task = (Task<object>)GetType().CallPrivateStaticGenericMethod("toObjectTask", ReflectionHelper.GetTypeFromGenericTask(returnType), _invocation.ReturnValue);

                    var result = await task.ConfigureAwait(false);
                    return CreateResult(result);
                }

                return CreateResult(_invocation.ReturnValue);
            }
            catch (Exception e)
            {
                return CreateExceptionResult(e);
            }
        }

        private static async Task<object> toObjectTask<T>(object task)
        {
            return await ((Task<T>)task).ConfigureAwait(false);
        }

        public IMethodInvocationResult CreateResult(object value)
        {
            return new MethodInvocationResult(value);
        }

        public IMethodInvocationResult CreateExceptionResult(Exception exception)
        {
            return new MethodInvocationResult(exception);
        }

        public Type ActualReturnType
        {
            get
            {
                return ReflectionHelper.GetActualType(MethodInfo.ReturnType);
            }
        }
    }
}