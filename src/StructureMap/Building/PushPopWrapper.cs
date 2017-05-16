﻿using System;
using System.Linq.Expressions;
using System.Reflection;
using StructureMap.Pipeline;

namespace StructureMap.Building
{
    public static class PushPopWrapper
    {
        public static readonly MethodInfo PushMethod = typeof(IBuildSession).GetMethod(nameof(IBuildSession.Push));
        public static readonly MethodInfo PopMethod = typeof(IBuildSession).GetMethod(nameof(IBuildSession.Pop));

        public static Expression WrapFunc(Type returnType, Instance instance, Expression inner)
        {
            var push = Expression.Call(Parameters.Session, PushMethod, Expression.Constant(instance));
            var block = Expression.Block(returnType, push, inner);
            var pop = Expression.Call(Parameters.Session, PopMethod);

            return Expression.TryFinally(block, pop);
        }
    }
}