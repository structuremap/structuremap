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
     * 4.) Decorate with T
     * 5.) Expression<Func<T, T>> -- supported
     * 6.) Func<T, TDecorator> where T : TDecorator, description
     * 7.) Expression<Func<IContext, T, T>> -- supported
     * 8.) Func<IContext, T, TDecorator>, description
     * 
     * 
     */

    public static class InterceptorFactory
    {
        public static IInterceptor ForAction<T>(string description, Action<T> action)
        {
            throw new NotImplementedException();
        }

        public static IInterceptor ForAction<T>(string description, Action<IContext, T> action)
        {
            throw new NotImplementedException();
        }

        public static IInterceptor ForFunc<T>(string description, Func<T, T> action)
        {
            throw new NotImplementedException();
        }

        public static IInterceptor ForFunc<T>(string description, Action<IContext, T, T> action)
        {
            throw new NotImplementedException();
        }

    }


    public static class Think
    {
        public static void Do()
        {
            Action<SomeTarget> act = x => x.Call();

            Expression<Action<SomeTarget>> expr = x => act(x);


            Debug.WriteLine(expr);
        }
    }

    public class SomeTarget
    {
        public void Call()
        {
        }
    }


    // These will go on Policies somehow
    // want one that can handle open generics
    public interface IInterceptorPolicy : IDescribed
    {
        IEnumerable<IInterceptor> DetermineInterceptors(Type concreteType);
    }


    public class DecoratorInterceptor<TDecorator, TInner> : IInterceptor where TDecorator : class, TInner
    {
        public DecoratorInterceptor(ConstructorInfo constructor)
        {
        }

        public string Description { get; private set; }
        public InterceptorRole Role { get; private set; }

        public Expression ToExpression(ParameterExpression session, ParameterExpression variable)
        {
            throw new NotImplementedException();
        }

        public Type Accepts { get; private set; }
        public Type Returns { get; private set; }
    }
}