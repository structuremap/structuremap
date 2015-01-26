using System;
using System.Collections.Generic;
using StructureMap.Pipeline;

namespace StructureMap.Building.Interception
{
    public interface IInterceptorPolicy : IDescribed
    {
        IEnumerable<IInterceptor> DetermineInterceptors(Type pluginType, Instance instance);
    }
}