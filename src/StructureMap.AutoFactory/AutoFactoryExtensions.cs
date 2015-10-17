using Castle.DynamicProxy;
using StructureMap.Configuration.DSL.Expressions;
using StructureMap.TypeRules;
using System;

namespace StructureMap.AutoFactory
{
    public static class AutoFactoryExtensions
    {
        private static readonly ProxyGenerator proxyGenerator = new ProxyGenerator();

        public static void CreateFactory<TPluginType>(this CreatePluginFamilyExpression<TPluginType> expression)
            where TPluginType : class
        {
            CreateFactory(expression, new DefaultAutoFactoryConventionProvider());
        }

        public static void CreateFactory<TPluginType>(this CreatePluginFamilyExpression<TPluginType> expression,
            IAutoFactoryConventionProvider conventionProvider)
            where TPluginType : class
        {
            var callback = CreateFactoryCallback<TPluginType>(conventionProvider);

            expression.Use(GetDescription<TPluginType>(), callback);
        }

        public static void CreateFactory<TPluginType, TConventionProvider>(
            this CreatePluginFamilyExpression<TPluginType> expression)
            where TPluginType : class
            where TConventionProvider : IAutoFactoryConventionProvider
        {
            var callback = CreateFactoryCallback<TPluginType, TConventionProvider>();

            expression.Use(GetDescription<TPluginType>(), callback);
        }

        private static string GetDescription<TPluginType>() where TPluginType : class
        {
            return "AutoFactory builder for " + typeof(TPluginType).GetFullName();
        }

        private static Func<IContext, TPluginType> CreateFactoryCallback<TPluginType>(IAutoFactoryConventionProvider conventionProvider)
            where TPluginType : class
        {
            return ctxt =>
            {
                var proxyFactory = new ProxyFactory<TPluginType>(proxyGenerator, ctxt, conventionProvider);

                return proxyFactory.Create();
            };
        }

        private static Func<IContext, TPluginType> CreateFactoryCallback<TPluginType, TConventionProvider>()
            where TPluginType : class
            where TConventionProvider : IAutoFactoryConventionProvider
        {
            return ctxt =>
            {
                var conventionProvider = ctxt.GetInstance<TConventionProvider>();

                var proxyFactory = new ProxyFactory<TPluginType>(proxyGenerator, ctxt, conventionProvider);

                return proxyFactory.Create();
            };
        }
    }
}