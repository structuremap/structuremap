using Castle.DynamicProxy;
using StructureMap.Building.Interception;
using StructureMap.TypeRules;
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

        private readonly string _description;

        public DynamicProxyInterceptor(IEnumerable<Type> interceptionBehaviorTypes) : this(interceptionBehaviorTypes.ToArray())
        {
        }

        public DynamicProxyInterceptor(params Type[] interceptionBehaviorTypes) : base(buildExpression(interceptionBehaviorTypes))
        {
            _description = buildDescription(interceptionBehaviorTypes);
        }

        public DynamicProxyInterceptor(IEnumerable<IInterceptionBehavior> interceptionBehaviors) : this(interceptionBehaviors.ToArray())
        {
        }

        private DynamicProxyInterceptor(IInterceptionBehavior[] interceptionBehaviors) : base(buildExpression(interceptionBehaviors))
        {
            _description = buildDescription(interceptionBehaviors.Select(b => b.GetType()));
        }

        private static Expression<Func<IContext, TPluginType, TPluginType>> buildExpression(IEnumerable<Type> interceptionBehaviorTypes)
        {
            return (context, instance) => proxyGenerator.CreateInterfaceProxyWithTarget(
                instance,
                wrapInterceptors(interceptionBehaviorTypes.Select(t => (IInterceptionBehavior)context.GetInstance(t)).ToArray())
            );
        }

        private static Expression<Func<TPluginType, TPluginType>> buildExpression(IEnumerable<IInterceptionBehavior> interceptionBehaviors)
        {
            return instance => proxyGenerator.CreateInterfaceProxyWithTarget(
                instance,
                wrapInterceptors(interceptionBehaviors.ToArray())
            );
        }

        private static Castle.DynamicProxy.IInterceptor wrapInterceptors(IInterceptionBehavior[] interceptionBehaviors)
        {
            foreach (var behavior in interceptionBehaviors)
            {
                if (!(behavior is ISyncInterceptionBehavior) &&
                    !(behavior is IAsyncInterceptionBehavior))
                {
                    throw new StructureMapException(
                        $"{behavior.GetType().GetFullName()} implements neither ISyncInterceptionBehavior nor IAsyncInterceptionBehavior");
                }
            }
            return new CastleInterceptor(interceptionBehaviors);
        }

        private static string buildDescription(IEnumerable<Type> interceptionBehaviorTypes)
        {
            return
                $"DynamicProxyInterceptor of {typeof(TPluginType).GetFullName()} with interception behaviors: {string.Join(", ", interceptionBehaviorTypes.Select(t => t.GetFullName()))}";
        }

        public override string Description => _description;
    }
}