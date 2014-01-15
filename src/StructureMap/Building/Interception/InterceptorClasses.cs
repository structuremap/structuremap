using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace StructureMap.Building.Interception
{
    /*
     * 1.) Expression<Action<T>> -- Supported
     * 2.) Expression<Action<IContext, T>> -- supported
     * 3.) Action<IContext, T>, description
     * 4.) Decorate with T -- do differently
     * 5.) Expression<Func<T, T>> -- supported
     * 6.) Func<T, TDecorator> where T : TDecorator, description
     * 7.) Expression<Func<IContext, T, T>> -- supported
     * 8.) Func<IContext, T, TDecorator>, description
     * 
     * 
     */


    // These will go on Policies somehow
    // want one that can handle open generics
    public interface IInterceptorPolicy : IDescribed
    {
        IEnumerable<IInterceptor> DetermineInterceptors(Type concreteType);
    }
}