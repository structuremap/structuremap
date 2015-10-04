using Castle.DynamicProxy;
using StructureMap.Building.Interception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace StructureMap.DynamicInterception
{
    public class DynamicProxyInterceptor<TPluginType> : FuncInterceptor<TPluginType>
        where TPluginType : class

    {
        private static readonly ProxyGenerator proxyGenerator = new ProxyGenerator();

        public DynamicProxyInterceptor(IEnumerable<Type> interceptionBehaviorTypes) : base(buildExpression(interceptionBehaviorTypes))
        {
        }

        public DynamicProxyInterceptor(params Type[] interceptionBehaviorTypes) : this((IEnumerable<Type>)interceptionBehaviorTypes)
        {
        }

        public DynamicProxyInterceptor(IEnumerable<IInterceptionBehavior> interceptionBehaviors) : base(buildExpression(interceptionBehaviors))
        {
        }

        private static Expression<Func<IContext, TPluginType, TPluginType>> buildExpression(IEnumerable<Type> interceptionBehaviorTypes)
        {
            return (context, instance) => proxyGenerator.CreateInterfaceProxyWithTarget(
                instance,
                interceptionBehaviorTypes
                    .Select(t => WrapInterceptorBehavior((IInterceptionBehavior)context.GetInstance(t)))
                    .ToArray()
            );
        }

        private static Expression<Func<TPluginType, TPluginType>> buildExpression(IEnumerable<IInterceptionBehavior> interceptionBehaviors)
        {
            return instance => proxyGenerator.CreateInterfaceProxyWithTarget(
                instance,
                interceptionBehaviors.Select(WrapInterceptorBehavior).ToArray()
            );
        }

        private static Castle.DynamicProxy.IInterceptor WrapInterceptorBehavior(IInterceptionBehavior behavior)
        {
            return new CastleWrapperInterceptor(behavior);
        }
    }
}