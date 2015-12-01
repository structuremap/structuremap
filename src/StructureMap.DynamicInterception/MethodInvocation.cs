using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace StructureMap.DynamicInterception
{
    internal class MethodInvocation : ISyncMethodInvocation, IAsyncMethodInvocation
    {
        private readonly IInvocation _invocation;
        private readonly Lazy<ReadOnlyCollection<IArgument>> _arguments;
        private readonly Lazy<IDictionary<string, IArgument>> _argumentMap;

        public MethodInvocation(IInvocation invocation)
        {
            _invocation = invocation;
            _arguments = new Lazy<ReadOnlyCollection<IArgument>>(() => invocation.Method.GetParameters()
                                                                                 .Zip(invocation.Arguments, (info, value) => new { info, value })
                                                                                 .Select((t, i) => new Argument(invocation, i, t.value, t.info))
                                                                                 .ToList<IArgument>()
                                                                                 .AsReadOnly());
            _argumentMap = new Lazy<IDictionary<string, IArgument>>(() => _arguments.Value.ToDictionary(a => a.ParameterInfo.Name));
        }

        public IList<IArgument> Arguments
        {
            get { return _arguments.Value; }
        }

        public IArgument GetArgument(string name)
        {
            IArgument result;
            return _argumentMap.Value.TryGetValue(name, out result) ? result : null;
        }

        public object TargetInstance
        {
            get { return _invocation.InvocationTarget; }
        }

        public MethodInfo MethodInfo
        {
            get { return _invocation.Method; }
        }

        public MethodInfo InstanceMethodInfo
        {
            get { return _invocation.MethodInvocationTarget; }
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
                    var result = ReflectionHelper.GetTaskResult(ReflectionHelper.GetTypeFromGenericTask(returnType),
                        _invocation.ReturnValue);

                    return CreateResult(result);
                }

                return CreateResult(_invocation.ReturnValue);
            }
            catch (Exception e)
            {
                return CreateExceptionResult(e);
            }
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
                    return CreateResult(null);
                }

                if (ReflectionHelper.IsGenericTask(returnType))
                {
                    var result = await ReflectionHelper.GetTaskResultAsync(ReflectionHelper.GetTypeFromGenericTask(returnType), _invocation.ReturnValue).ConfigureAwait(false);
                    return CreateResult(result);
                }

                return CreateResult(_invocation.ReturnValue);
            }
            catch (Exception e)
            {
                return CreateExceptionResult(e);
            }
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