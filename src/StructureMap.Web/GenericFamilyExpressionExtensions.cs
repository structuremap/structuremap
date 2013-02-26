using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StructureMap.Configuration.DSL.Expressions;

namespace StructureMap.Web
{
    public static class GenericFamilyExpressionExtensions
    {
        /// <summary>
        /// Convenience method to mark a PluginFamily as a Hybrid lifecycle
        /// </summary>
        /// <returns></returns>
        public static GenericFamilyExpression HybridHttpOrThreadLocalScoped(this GenericFamilyExpression expression)
        {
            return expression.LifecycleIs(WebLifecycles.Hybrid);
        }

        /// <summary>
        /// Convenience method to mark a PluginFamily as HttpContext scoped
        /// </summary>
        /// <returns></returns>
        public static GenericFamilyExpression HttpContextScoped(this GenericFamilyExpression expression)
        {
            return expression.LifecycleIs(WebLifecycles.HttpContext);
        }
    }
}
