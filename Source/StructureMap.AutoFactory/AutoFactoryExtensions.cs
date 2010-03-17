using System;
using StructureMap.Configuration.DSL.Expressions;

namespace StructureMap.AutoFactory
{
    public static class AutoFactoryExtensions
    {
        public static void CreateFactory<PLUGINTYPE>(this CreatePluginFamilyExpression<PLUGINTYPE> expression)
        {
            var factoryBuilder = CreateFactoryCallback<PLUGINTYPE>();

            expression.Use(factoryBuilder);
        }

        private static Func<PLUGINTYPE> CreateFactoryCallback<PLUGINTYPE>()
        {
            return null;
        }
    }
}