using System;
using StructureMap.Graph;

namespace StructureMap.Configuration.DSL
{
    public static class ExpressionValidator
    {
        public static ValidateExpression ValidatePluggabilityOf(Type pluggedType)
        {
            return new ValidateExpression(pluggedType);
        }

        #region Nested type: ValidateExpression

        public class ValidateExpression
        {
            private readonly Type _pluggedType;

            public ValidateExpression(Type pluggedType)
            {
                _pluggedType = pluggedType;
            }

            public void IntoPluginType(Type pluginType)
            {
                if (!TypeRules.CanBeCast(pluginType, _pluggedType))
                {
                    throw new StructureMapException(
                        303,
                        TypePath.GetAssemblyQualifiedName(_pluggedType),
                        TypePath.GetAssemblyQualifiedName(pluginType));
                }
            }
        }

        #endregion
    }
}