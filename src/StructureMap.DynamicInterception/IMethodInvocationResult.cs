using System;

namespace StructureMap.DynamicInterception
{
    public interface IMethodInvocationResult
    {
        bool Successful { get; }

        object ReturnValue { get; }

        Exception Exception { get; }
    }
}