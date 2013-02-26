using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StructureMap.Configuration.DSL.Expressions;
using StructureMap.Pipeline;

namespace StructureMap.Web
{
    public static class CreatePluginFamilyExpressionExtensions
    {
        /// <summary>
        /// Convenience method to mark a PluginFamily as a Hybrid lifecycle
        /// </summary>
        /// <returns></returns>
        public static CreatePluginFamilyExpression<TPluginType> HybridHttpOrThreadLocalScoped<TPluginType>(this CreatePluginFamilyExpression<TPluginType> source)
        {
            return source.LifecycleIs(WebLifecycles.Hybrid);
        }

        /// <summary>
        /// Convenience method to mark a PluginFamily as HttpContext scoped
        /// </summary>
        /// <returns></returns>
        public static CreatePluginFamilyExpression<TPluginType> HttpContextScoped<TPluginType>(this CreatePluginFamilyExpression<TPluginType> source)
        {
            return source.LifecycleIs(WebLifecycles.HttpContext);
        }
    }
}
