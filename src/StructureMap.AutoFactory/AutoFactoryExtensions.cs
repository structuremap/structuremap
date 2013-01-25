using System;
using Castle.DynamicProxy;
using StructureMap.Configuration.DSL.Expressions;

namespace StructureMap.AutoFactory
{
    public static class AutoFactoryExtensions
    {
        private static readonly ProxyGenerator proxyGenerator = new ProxyGenerator();

        public static void CreateFactory<TPluginType>(this CreatePluginFamilyExpression<TPluginType> expression) 
            where TPluginType : class
        {
            var callback = CreateFactoryCallback<TPluginType>();

            expression.Use(callback);
        }

        private static Func<IContext, TPluginType> CreateFactoryCallback<TPluginType>() 
            where TPluginType : class
        {
            return ctxt => 
            {
                var proxyFactory = new ProxyFactory<TPluginType>(proxyGenerator, ctxt);

                return proxyFactory.Create();
            };
        }
    }
}