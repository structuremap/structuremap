using System;
using StructureMap.Attributes;
using StructureMap.Graph;

namespace StructureMap.Interceptors
{
    [Obsolete]
    public interface IInterceptorChainBuilder
    {
        [Obsolete]
        InterceptionChain Build(InstanceScope scope);
    }
}