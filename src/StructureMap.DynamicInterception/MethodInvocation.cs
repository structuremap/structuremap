using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StructureMap.DynamicInterception
{
    internal class MethodInvocation : IMethodInvocation
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
    }
}