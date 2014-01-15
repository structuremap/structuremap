using System;
using System.Collections.Generic;

namespace StructureMap.Building.Interception
{
    public interface IInterceptorPolicy : IDescribed
    {
        IEnumerable<IInterceptor> DetermineInterceptors(Type returnedType);
    }
}