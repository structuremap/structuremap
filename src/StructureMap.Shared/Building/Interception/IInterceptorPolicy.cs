using System;
using System.Collections.Generic;
using StructureMap.Pipeline;

namespace StructureMap.Building.Interception
{
    // SAMPLE: IInterceptorPolicy
    /// <summary>
    /// User defined policy to conventionally attach interceptors
    /// to any pluginType/instance combination in a StructureMap
    /// container
    /// </summary>
    public interface IInterceptorPolicy : IDescribed
    {
        /// <summary>
        /// Determine what (if any) interceptors should be attached to the
        /// given instance and pluginType
        /// </summary>
        /// <param name="pluginType">The Type that is being requested</param>
        /// <param name="instance">The Instance being built</param>
        /// <returns></returns>
        IEnumerable<IInterceptor> DetermineInterceptors(Type pluginType, Instance instance);
    }
    // ENDSAMPLE
}