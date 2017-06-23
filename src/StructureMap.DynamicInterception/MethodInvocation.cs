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
        private readonly IInterceptionBehavior[] _interceptors;
        private readonly int _currentIndex;
        private readonly Lazy<ReadOnlyCollection<IArgument>> _arguments;
        private readonly Lazy<IDictionary<string, IArgument>> _argumentMap;

        public MethodInvocation(IInvocation invocation, IInterceptionBehavior[] interceptors, int currentIndex = 0)
        {
            _invocation = invocation;
            _interceptors = interceptors;
            _currentIndex = currentIndex;
            _arguments = new Lazy<ReadOnlyCollection<IArgument>>(() => invocation.Method.GetParameters()
                .Zip(invocation.Arguments, (info, value) => new { info, value })
                .Select((t, i) => new Argument(invocation, i, t.value, t.info))
                .ToList<IArgument>()
                .AsReadOnly()
            );
            _argumentMap = new Lazy<IDictionary<string, IArgument>>(() => _arguments.Value
                .ToDictionary(a => a.ParameterInfo.Name)
            );
        }

        private MethodInvocation(IInvocation invocation, IInterceptionBehavior[] interceptors, int currentIndex,
            Lazy<ReadOnlyCollection<IArgument>> arguments, Lazy<IDictionary<string, IArgument>> argumentMap)
        {
            _invocation = invocation;
            _interceptors = interceptors;
            _currentIndex = currentIndex;
            _arguments = arguments;
            _argumentMap = argumentMap;
        }

        private MethodInvocation GetNextInvocation()
        {
            return new MethodInvocation(_invocation, _interceptors, _currentIndex + 1, _arguments, _argumentMap);
        }

        public IList<IArgument> Arguments => _arguments.Value;

        public IArgument GetArgument(string name)
        {
            IArgument result;
            return _argumentMap.Value.TryGetValue(name, out result) ? result : null;
        }

        public object TargetInstance => _invocation.InvocationTarget;

        public MethodInfo MethodInfo => _invocation.Method;

        public MethodInfo InstanceMethodInfo => _invocation.MethodInvocationTarget;

        public IMethodInvocationResult InvokeNext()
        {
            try
            {
                if (_currentIndex >= _interceptors.Length)
                {
                    var result = InstanceMethodInfo.Invoke(TargetInstance, _invocation.Arguments);

                    var actualResult = ReflectionHelper.IsTask(MethodInfo.ReturnType)
                        ? ReflectionHelper.GetResultFromTask(ActualReturnType, (Task)result)
                        : result;

                    return CreateResult(actualResult);
                }

                var interceptionBehavior = _interceptors[_currentIndex];

                var asyncBehavior = interceptionBehavior as IAsyncInterceptionBehavior;
                if (asyncBehavior != null)
                {
                    var invocationResultTask = asyncBehavior.InterceptAsync(GetNextInvocation());
                    return invocationResultTask.GetAwaiter().GetResult();
                }

                return ((ISyncInterceptionBehavior)interceptionBehavior).Intercept(GetNextInvocation());
            }
            catch (TargetInvocationException e)
            {
                return CreateExceptionResult(e.InnerException);
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
                if (_currentIndex >= _interceptors.Length)
                {
                    var result = InstanceMethodInfo.Invoke(TargetInstance, _invocation.Arguments);

                    if (ReflectionHelper.IsTask(MethodInfo.ReturnType))
                    {
                        await ((Task)result).ConfigureAwait(false);
                        return CreateResult(ReflectionHelper.GetResultFromTask(ActualReturnType, (Task)result));
                    }

                    return CreateResult(result);
                }

                var interceptionBehavior = _interceptors[_currentIndex];

                var asyncBehavior = interceptionBehavior as IAsyncInterceptionBehavior;
                if (asyncBehavior != null)
                {
                    return await asyncBehavior.InterceptAsync(GetNextInvocation()).ConfigureAwait(false);
                }

                return ((ISyncInterceptionBehavior)interceptionBehavior).Intercept(GetNextInvocation());
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

        public Type ActualReturnType => ReflectionHelper.GetActualType(MethodInfo.ReturnType);
    }
}