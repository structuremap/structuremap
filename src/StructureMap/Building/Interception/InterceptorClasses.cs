using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace StructureMap.Building.Interception
{
    /*
     * 1.) Expression<Action<T>>
     * 2.) Expression<Action<IContext, T>>
     * 3.) Action<IContext, T>, description
     * 4.) Decorate with T
     * 5.) Expression<Func<T, TDecorator>> where T : TDecorator
     * 6.) Func<T, TDecorator> where T : TDecorator, description
     * 7.) Expression<Func<IContext, T, TDecorator>>
     * 8.) Func<IContext, T, TDecorator>, description
     * 
     * 
     */

    public static class Think
    {
        public static void Do()
        {
            Expression value = Expression.Constant("horse");
            var variable = Expression.Variable(typeof (string), "x");
            var assignment = Expression.Assign(variable, value);

            Expression<Action<SomeTarget>> expr = x => x.Call();
            //var action = expr.Compile();
            //Expression<Action<SomeTarget>> intercepted = s => action(s);

            var later = Expression.Parameter(typeof (SomeTarget), "later");


        }
    }

    public class SomeTarget
    {
        public void Call()
        {
            
        }
    }


    public class InterceptionPlan : IBuildPlan
    {
        private readonly Type _pluginType;
        private readonly IBuildPlan _inner;
        private readonly IEnumerable<IInterceptor> _interceptors;

        public InterceptionPlan(Type pluginType, IBuildPlan inner, IEnumerable<IInterceptor> interceptors)
        {
            _pluginType = pluginType;
            _inner = inner;
            _interceptors = interceptors;
        }

        public object Build(IBuildSession session)
        {
            throw new NotImplementedException();
        }

        public Expression ToExpression(ParameterExpression session)
        {
            var label = Expression.Label(_pluginType);
            

            var inner = _inner.ToExpression(session);
            var list = new List<Expression>();
            
            var activators = _interceptors.Where(x => x.Role == InterceptorRole.Activates);

            var variable = Expression.Variable(_inner.ReturnedType, "x");
            Expression returnTarget = variable;

            var assignment = Expression.Assign(variable, inner);
            list.Add(assignment);

            var activations = activators.Select(x => x.ToExpression(session, variable));
            list.AddRange(activations);



            list.Add(Expression.Return(label, returnTarget, _pluginType));
            list.Add(Expression.Label(label));

            return Expression.Block(list.ToArray());

        }

        public Type ReturnedType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Description { get; private set; }
    }

    // These will go on Policies somehow
    // want one that can handle open generics
    public interface IInterceptorPolicy : IDescribed
    {
        IEnumerable<IInterceptor> DetermineInterceptors(Type concreteType);
    }

    public interface IInterceptor : IDescribed
    {
        InterceptorRole Role { get; }

        // If it's an "Activates", gather them up in a batch
        // If it's "Decorates", nest.
        // Do it in Instance
        Expression ToExpression(ParameterExpression session, ParameterExpression variable);

        Type Accepts { get; }
        Type Returns { get; }
    }

    public class ContextualActivatorInterceptor<T> : IInterceptor
    {
        private readonly Expression<Action<IContext, T>> _expression;

        public ContextualActivatorInterceptor(Expression<Action<IContext, T>> expression)
        {
            _expression = expression;
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

    public enum InterceptorRole
    {
        Activates,
        Decorates
    }

}