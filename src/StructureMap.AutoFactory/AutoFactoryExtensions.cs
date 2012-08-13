using System;
using Castle.DynamicProxy;
using StructureMap.Configuration.DSL.Expressions;

namespace StructureMap.AutoFactory
{
    public static class AutoFactoryExtensions
    {
        private static readonly ProxyGenerator proxyGenerator = new ProxyGenerator();

        public static void CreateFactory<PLUGINTYPE>(this CreatePluginFamilyExpression<PLUGINTYPE> expression) 
            where PLUGINTYPE : class
        {
            var callback = CreateFactoryCallback<PLUGINTYPE>();

            expression.Use(callback);
        }

        private static Func<IContext, PLUGINTYPE> CreateFactoryCallback<PLUGINTYPE>() 
            where PLUGINTYPE : class
        {
            return ctxt => 
            {
                var proxyFactory = new ProxyFactory<PLUGINTYPE>(proxyGenerator, ctxt);

                return proxyFactory.Create();
            };
        }
    }
}