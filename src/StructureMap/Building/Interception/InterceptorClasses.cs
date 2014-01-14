using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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

    public class BlockPlan
    {
        private readonly List<Expression> _expressions = new List<Expression>();
        private readonly List<ParameterExpression> _variables = new List<ParameterExpression>(); 

        public static BlockPlan operator +(BlockPlan plan, Expression expression)
        {
            plan._expressions.Add(expression);

            return plan;
        }

        public void Add(params Expression[] expressions)
        {
            _expressions.AddRange(expressions);
        }

        public BlockExpression ToExpression()
        {
            return Expression.Block(_variables, _expressions.ToArray());
        }

        public void AddVariable(ParameterExpression variable)
        {
            _variables.Add(variable);
        }

        public void AddVariables(IEnumerable<ParameterExpression> variables)
        {
            _variables.AddRange(variables);
        }
    }

}