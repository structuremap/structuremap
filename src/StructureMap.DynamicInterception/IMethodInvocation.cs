using System;
using System.Collections.Generic;
using System.Reflection;

namespace StructureMap.DynamicInterception
{
    public interface IMethodInvocation
    {
        IList<IArgument> Arguments { get; }

        IArgument GetArgument(string name);

        MethodInfo MethodInfo { get; }

        Type ActualReturnType { get; }

        IMethodInvocationResult CreateResult(object value);

        IMethodInvocationResult CreateExceptionResult(Exception e);
    }
}